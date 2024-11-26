using CarChanger.Common;
using CarChanger.Game.Components;
using DV;
using DV.ThingTypes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game
{
    internal class CommsRadioCarChanger : MonoBehaviour, ICommsRadioMode
    {
        private enum State
        {
            PointAtCar,
            CarSelected,
            MustOverride
        }

        private static Vector3 s_highlightExtra = new Vector3(0.25f, 0.8f, 0f);

        public CommsRadioController Controller = null!;
        public CommsRadioDisplay Display = null!;
        public Transform SignalOrigin = null!;

        private State _state;
        private int _mask;
        private RaycastHit _hit;
        private GameObject _highlighter = null!;
        private Renderer _highlighterRender = null!;
        private TrainCar _pointedCar = null!;
        private TrainCar _selectedCar = null!;
        private List<ModelConfig> _configs = null!;
        private List<AppliedChange> _applied = null!;
        private Dictionary<TrainCarLivery, int> _currentIndices = new Dictionary<TrainCarLivery, int>();
        private AudioClip _confirmSound = null!;
        private AudioClip _cancelSound = null!;
        private AudioClip _swapSound = null!;
        private AudioClip _successSound = null!;

        public ButtonBehaviourType ButtonBehaviour { get; private set; }

        private void Awake()
        {
            SignalOrigin = Controller.laserBeam.transform;
            Display = Controller.cargoLoaderControl.display;

            _mask = LayerMask.GetMask(new string[] { "Train_Big_Collider" });
            _highlighter = Instantiate(Controller.cargoLoaderControl.trainHighlighter);
            _highlighter.SetActive(false);
            _highlighterRender = _highlighter.GetComponentInChildren<MeshRenderer>(true);

            _confirmSound = CarChangerMod.SoundCache["CommsRadio_Confirm_01"];
            _cancelSound = CarChangerMod.SoundCache["CommsRadio_Cancel_01"];
            _swapSound = CarChangerMod.SoundCache["CommsRadio_Select_01"];
            _successSound = CarChangerMod.SoundCache["CommsRadio_RerailDrop_01"];
        }

        private void OnDestroy()
        {
            if (UnloadWatcher.isUnloading)
            {
                return;
            }

            if (_highlighter != null)
            {
                Destroy(_highlighter.gameObject);
            }
        }

        public Color GetLaserBeamColor()
        {
            return Color.HSVToRGB(Time.time % 1.0f, 1.0f, 1.0f);
        }

        public void SetStartingDisplay()
        {
            Display.SetDisplay("Car Changer", Localization.RadioBegin);
        }

        public void Enable()
        {
            SetStartingDisplay();
        }

        public void Disable()
        {
            ClearHighlightCar();
            ButtonBehaviour = ButtonBehaviourType.Regular;
            _state = State.PointAtCar;
            _pointedCar = null!;
            _selectedCar = null!;
            _configs = null!;
            _applied = null!;
        }

        public void OnUpdate()
        {
            Controller.laserBeam.SetBeamColor(GetLaserBeamColor());

            // Always update where we are pointing at.
            if (Physics.Raycast(SignalOrigin.position, SignalOrigin.forward, out _hit, 100f, _mask))
            {
                PointToCar(TrainCar.Resolve(_hit.transform.root));
            }
            else
            {
                PointToCar(null!);
            }

            switch (_state)
            {
                case State.PointAtCar:
                    // If we are pointing at a car, display its info.
                    if (_pointedCar != null)
                    {
                        Display.SetContent($"{Localization.GetLocalizedName(_pointedCar.carLivery)} ({_pointedCar.ID})");
                    }
                    else
                    {
                        SetStartingDisplay();
                    }
                    break;
                default:
                    break;
            }
        }

        public void OnUse()
        {
            switch (_state)
            {
                case State.PointAtCar:
                    PointAtCarUse();
                    return;
                case State.CarSelected:
                    CarSelectedUse();
                    return;
                case State.MustOverride:
                    MustOverrideUse();
                    return;
                default:
                    return;
            }
        }

        private void PointAtCarUse()
        {
            if (!_pointedCar)
            {
                PlayCancel();
                ButtonBehaviour = ButtonBehaviourType.Regular;
                return;
            }

            // Select the car we are pointing at.
            SelectCar();
        }

        private void CarSelectedUse()
        {
            // Not pointing at anything, cancel.
            if (!_pointedCar)
            {
                PlayCancel();
                ButtonBehaviour = ButtonBehaviourType.Regular;
                _state = State.PointAtCar;
                _selectedCar = null!;
                _configs = null!;
                return;
            }

            // Pointing at new car, change selection.
            if (_pointedCar != _selectedCar)
            {
                SelectCar();
                return;
            }

            // Vehicle has no configs available.
            if (_configs == null)
            {
                return;
            }

            var config = GetCurrentConfig();

            // Disable change.
            if (_applied.TryFind(x => x.Config == config, out var change))
            {
                UnapplyChange(change);
                return;
            }

            // Can combine the new change with the others.
            if (ModelConfig.CanCombine(config, _applied.Select(x => x.Config)!))
            {
                ApplyConfig(config);
                return;
            }

            // Cannot combine, so go into override mode.
            _state = State.MustOverride;
            Display.SetContentAndAction(Localization.IncompatibleModification, Localization.Override);
            PlayCancel();
        }

        private void MustOverrideUse()
        {
            _state = State.CarSelected;

            // Pointing at anything else other than the target cancels override.
            if (!_pointedCar || _pointedCar != _selectedCar)
            {
                DisplayCurrentConfig();
                PlayCancel();
                return;
            }

            var config = GetCurrentConfig();

            // Disable all conflicting changes.
            for (int i = 0; i < _applied.Count; i++)
            {
                if (!ModelConfig.CanCombine(_applied[i].Config!, config))
                {
                    Destroy(_applied[i]);
                    _applied.RemoveAt(i);
                    i--;
                }
            }

            // Apply the config with delay due to same frame shenanigans.
            StartCoroutine(DelayedApplyConfig(config));
        }

        public bool ButtonACustomAction()
        {
            switch (_state)
            {
                case State.CarSelected:
                    if (_configs != null)
                    {
                        // Scroll through the configs.
                        _currentIndices[_selectedCar.carLivery] = Helpers.Wrap(_currentIndices[_selectedCar.carLivery] + 1, _configs.Count);
                        DisplayCurrentConfig();
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        public bool ButtonBCustomAction()
        {
            switch (_state)
            {
                case State.CarSelected:
                    if (_configs != null)
                    {
                        // Scroll through the configs.
                        _currentIndices[_selectedCar.carLivery] = Helpers.Wrap(_currentIndices[_selectedCar.carLivery] - 1, _configs.Count);
                        DisplayCurrentConfig();
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        public void OverrideSignalOrigin(Transform signalOrigin)
        {
            SignalOrigin = signalOrigin;
        }

        private void HighlightCar(TrainCar car, Material highlightMaterial)
        {
            _highlighterRender.material = highlightMaterial;
            _highlighter.transform.localScale = car.Bounds.size + s_highlightExtra;

            // Move the centre of the highlight up.
            Vector3 b = car.transform.up * (_highlighter.transform.localScale.y * 0.5f);
            Vector3 b2 = car.transform.forward * car.Bounds.center.z;
            Vector3 position = car.transform.position + b + b2;

            // Move the actual highlight object to match the car.
            _highlighter.transform.SetPositionAndRotation(position, car.transform.rotation);
            _highlighter.SetActive(true);
            _highlighter.transform.SetParent(car.transform, true);
        }

        private void ClearHighlightCar()
        {
            if (_pointedCar != null)
            {
                _pointedCar.OnDestroyCar -= OnPointedCarDestroyed;
                _pointedCar = null!;
            }

            if (_highlighter != null)
            {
                _highlighter.SetActive(false);
                _highlighter.transform.SetParent(null);
            }
        }

        private void PointToCar(TrainCar car)
        {
            // Only do this if the pointed car has changed.
            if (_pointedCar != car)
            {
                // Unregister callbacks if the car is not gone.
                if (_pointedCar != null)
                {
                    _pointedCar.OnDestroyCar -= OnPointedCarDestroyed;
                }

                // If we are in fact pointing at a new car...
                if (car != null)
                {
                    // ...change the pointed car and highlight the new one, or...
                    _pointedCar = car;
                    _pointedCar.OnDestroyCar += OnPointedCarDestroyed;
                    HighlightCar(_pointedCar, Controller.cargoLoaderControl.validMaterial);
                }
                else
                {
                    // ...else, clear the highlight.
                    _pointedCar = null!;
                    ClearHighlightCar();
                }

                // Play a sound to show it changed.
                CommsRadioController.PlayAudioFromRadio(Controller.cargoLoaderControl.hoverOverCar, transform);
            }
        }

        private void SelectCar()
        {
            // Check if we are selecting something new.
            bool soundType = _selectedCar != _pointedCar;
            _selectedCar = _pointedCar;
            _state = State.CarSelected;
            ButtonBehaviour = ButtonBehaviourType.Override;

            // Check if configs exist for this livery.
            if (ChangeManager.LoadedConfigs.TryGetValue(_selectedCar.carLivery, out var configs) && configs.Count > 0)
            {
                _configs = configs;
                _applied = _selectedCar.GetAppliedChanges();

                // Get the cached current index, for consistency when moving between selected cars.
                // Allows for fast swapping of multiple for the same.
                if (!_currentIndices.TryGetValue(_selectedCar.carLivery, out int lastIndex))
                {
                    _currentIndices.Add(_selectedCar.carLivery, 0);
                }

                _currentIndices[_selectedCar.carLivery] = Helpers.Wrap(lastIndex, configs.Count);
                DisplayCurrentConfig();

                if (soundType)
                {
                    PlayNewSelection();
                }
                else
                {
                    PlayConfirm();
                }

                return;
            }
            else
            {
                _configs = null!;
                Display.SetContentAndAction($"{Localization.GetLocalizedName(_selectedCar.carLivery)} ({_selectedCar.ID})\n{Localization.NoModifications}");
                PlayCancel();
            }
        }

        private ModelConfig GetCurrentConfig() => _configs[_currentIndices[_selectedCar.carLivery]];

        private void DisplayCurrentConfig()
        {
            DisplayConfig(GetCurrentConfig());
        }

        private void DisplayConfig(ModelConfig config)
        {
            // Display config in format:
            // LIVERY (ID)
            // CONFIG NAME
            //
            // ENABLE/DISABLE
            Display.SetContentAndAction(
                $"{Localization.GetLocalizedName(_selectedCar.carLivery)} ({_selectedCar.ID})\n{Localization.GetLocalizedName(config)}",
                _applied.Any(x => x != null && x.Config == config) ? Localization.Disable : Localization.Enable);
        }

        private void ApplyConfig(ModelConfig config)
        {
            var change = AppliedChange.AddChange(_selectedCar, config);
            change.OnApply += QueueDisplayUpdate;
            PlaySuccess();
        }

        private void QueueDisplayUpdate(AppliedChange change)
        {
            _applied = _selectedCar.GetAppliedChanges();
            DisplayCurrentConfig();
            change.OnApply -= QueueDisplayUpdate;
        }

        private void UnapplyChange(AppliedChange change)
        {
            _applied.Remove(change);
            Destroy(change);
            DisplayCurrentConfig();
            PlaySuccess();
        }

        private System.Collections.IEnumerator DelayedApplyConfig(ModelConfig modelConfig)
        {
            // Wait for the next frame.
            yield return null;
            ApplyConfig(modelConfig);
            PlaySuccess();
        }

        private System.Collections.IEnumerator DelayedApplyConfig(ModelConfig modelConfig, int frames)
        {
            while (frames > 0)
            {
                yield return null;
                frames--;
            }

            ApplyConfig(modelConfig);
            PlaySuccess();
        }

        private void OnPointedCarDestroyed(TrainCar destroyedCar)
        {
            if (destroyedCar != null)
            {
                destroyedCar.OnDestroyCar -= OnPointedCarDestroyed;
            }

            ClearHighlightCar();
        }

        #region Sound

        private void PlayConfirm()
        {
            CommsRadioController.PlayAudioFromRadio(_confirmSound, transform);
        }

        private void PlayCancel()
        {
            CommsRadioController.PlayAudioFromRadio(_cancelSound, transform);
        }

        private void PlayNewSelection()
        {
            CommsRadioController.PlayAudioFromRadio(_swapSound, transform);
        }

        private void PlaySuccess()
        {
            CommsRadioController.PlayAudioFromRadio(_successSound, transform);
        }

        #endregion
    }
}
