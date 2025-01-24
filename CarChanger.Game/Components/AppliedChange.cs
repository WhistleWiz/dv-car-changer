using CarChanger.Common;
using CarChanger.Common.Components;
using CarChanger.Common.Configs;
using CarChanger.Game.HeadlightChanges;
using CarChanger.Game.InteractablesChanges;
using CarChanger.Game.InteriorChanges;
using DV.Customization.Paint;
using DV.Simulation.Brake;
using DV.Wheels;
using LocoSim.Implementations.Wheels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game.Components
{
    internal partial class AppliedChange : MonoBehaviour
    {
        private const string InteriorLODNamingEnd = " LOD CHANGE";

        public TrainCar TrainCar = null!;
        public ModelConfig? Config = null;
        public event Action<AppliedChange>? OnApply = null;
        public MaterialHolder MatHolder = null!;

        private bool _changeApplied = false;
        private bool _applying = false;
        private List<GameObject> _originalBody = new List<GameObject>();
        private List<GameObject> _originalInteriorLod = new List<GameObject>();
        private GameObject _body = null!;
        private GameObject _interiorLod = null!;
        private bool _bodyHidden = false;
        private bool _interiorLodHidden = false;
        private HeadlightChanger? _frontHeadlights = null;
        private HeadlightChanger? _rearHeadlights = null;
        private IInteriorChanger? _interior = null;
        private IInteractablesChanger? _interactables = null;
        private ExplosionModelHandler? _explosionHandler = null;
        private ExplosionModelHandler? _explosionHandlerInteriorLod = null;
        private ColliderHolder? _colliderHolder = null;
        private BogieChanger? _bogieChanger = null;

        private void Awake()
        {
            TrainCar = TrainCar.Resolve(gameObject);

            if (!TrainCar)
            {
                Debug.LogError("Could not find TrainCar for AppliedChange! This should never happen BTW.");
                Destroy(this);
            }
        }

        private void Start()
        {
            ApplyChange();

            if (TrainCar != null)
            {
                if (TrainCar.PaintExterior != null)
                {
                    TrainCar.PaintExterior.OnThemeChanged += ReapplyForPaint;
                }
                if (TrainCar.PaintInterior != null)
                {
                    TrainCar.PaintInterior.OnThemeChanged += ReapplyForPaint;
                }
            }
        }

        private void OnDestroy()
        {
            // In case the whole object is killed, don't bother with
            // removing the actual modification.
            if (!TrainCar || !gameObject)
            {
                return;
            }

            ReturnToDefault();

            if (TrainCar != null)
            {
                if (TrainCar.PaintExterior != null)
                {
                    TrainCar.PaintExterior.OnThemeChanged -= ReapplyForPaint;
                }
                if (TrainCar.PaintInterior != null)
                {
                    TrainCar.PaintInterior.OnThemeChanged -= ReapplyForPaint;
                }
            }
        }

        private void ApplyChange()
        {
            StartCoroutine(ApplyChangeRoutine(0));
        }

        private System.Collections.IEnumerator ApplyChangeRoutine(int delay)
        {
            if (Config == null)
            {
                CarChangerMod.Warning($"No config on AppliedChange '{name}'!");
                yield break;
            }

            while (_applying)
            {
                yield return null;
            }

            _applying = true;

            if (_changeApplied)
            {
                ReturnToDefault();
            }

            while (delay > 0)
            {
                delay--;
                yield return null;
            }

            LogChange();

            _originalBody.Clear();
            _originalBody = GetOriginalBody();
            _originalInteriorLod.Clear();
            _originalInteriorLod = GetOriginalInteriorLod();

            switch (Config)
            {
                case WagonConfig wagon:
                    ApplyWagon(wagon);
                    break;
                case PassengerConfig pax:
                    ApplyPassenger(pax);
                    break;
                case CabooseConfig caboose:
                    ApplyCaboose(caboose);
                    break;

                case LocoDE2480Config de2:
                    ApplyDE2480(de2);
                    break;
                case LocoDE6860Config de6:
                    ApplyDE6860(de6);
                    break;
                case LocoDH4670Config dh4:
                    ApplyDH4670(dh4);
                    break;
                case LocoDM3540Config dm3:
                    ApplyDM3540(dm3);
                    break;
                case LocoDM1U150Config dm1u:
                    ApplyDM1U150(dm1u);
                    break;

                case LocoS060440Config s060:
                    ApplyS060440(s060);
                    break;
                case LocoS282730AConfig s282A:
                    ApplyS282730A(s282A);
                    break;
                case LocoS282730BConfig s282B:
                    ApplyS282730B(s282B);
                    break;

                case LocoBE2260Config be2:
                    ApplyBE2260(be2);
                    break;

                case LocoDE6860SlugConfig de6Slug:
                    ApplyDE6860Slug(de6Slug);
                    break;
                case LocoHandcarConfig handcar:
                    ApplyHandcar(handcar);
                    break;

                case CustomCarConfig custom:
                    ApplyCustomCar(custom);
                    break;

                case ModificationGroupConfig group:
                    ApplyGroup(group);
                    break;
                default:
                    yield break;
            }

            // If the applied change resulted in a body GameObject, see if it explodes.
            if (_body != null)
            {
                _explosionHandler = CarChangerExplosionManager.PrepareExplosionHandler(_body, MatHolder);
            }
            // Same with the interior LOD, unfortunately they have to be separate...
            if (_interiorLod != null)
            {
                _explosionHandlerInteriorLod = CarChangerExplosionManager.PrepareExplosionHandler(_interiorLod, MatHolder);
            }

            _colliderHolder?.Apply();

            // Handle other mods.
            ModIntegrations.ZCouplers.HandleBuffersToggled(TrainCar);

            Config.Applied(TrainCar.gameObject);
            OnApply?.Invoke(this);

            if (Config.ForcePrefabReloadOnApply)
            {
                TrainCar.ForceRefreshLoadedPrefabs();
            }

            _changeApplied = true;
            _applying = false;
        }

        private List<GameObject> GetOriginalBody()
        {
            List<GameObject> result;

            // Get the body object at the path.
            // Locos have a special path to ensure everything works fine.
            switch (Config)
            {
                case PassengerConfig _:
                    return new List<GameObject>
                    {
                        transform.Find("CarPassenger/CarPassenger_LOD0").gameObject,
                        transform.Find("CarPassenger/CarPassenger_LOD1").gameObject,
                        transform.Find("CarPassenger/CarPassenger_LOD2").gameObject,
                        transform.Find("CarPassenger/CarPassenger_LOD3").gameObject
                    };
                case CabooseConfig _:
                    return new List<GameObject>
                    {
                        transform.Find("CarCaboose_exterior/CabooseExterior").gameObject,
                        transform.Find("CarCaboose_exterior/CabooseExterior_LOD1").gameObject,
                        transform.Find("CarCaboose_exterior/CabooseExterior_LOD2").gameObject,
                        transform.Find("CarCaboose_exterior/Caboose_LOD3").gameObject
                    };

                case LocoDE2480Config _:
                    return new List<GameObject>
                    {
                        transform.Find("LocoDE2_Body/ext 621_exterior").gameObject,
                        transform.Find("LocoDE2_Body/LocoShunterExterior_lod").gameObject
                    };
                case LocoDE6860Config _:
                    return new List<GameObject>
                    {
                        transform.Find("LocoDE6_Body/Body").gameObject
                    };
                case LocoDH4670Config _:
                    return new List<GameObject>
                    {
                        transform.Find("LocoDH4_Body/Body").gameObject
                    };
                case LocoDM3540Config _:
                    return new List<GameObject>
                    {
                        transform.Find("LocoDM3_Body/LocoDM3_exterior_LOD").gameObject
                    };
                case LocoDM1U150Config _:
                    return new List<GameObject>
                    {
                        transform.Find("LocoDM1U_Body/body").gameObject,
                        transform.Find("LocoDM1U_Body/bed").gameObject,
                    };

                case LocoS060440Config _:
                    return transform.Find("LocoS060_Body/Static").AllChildGOsExcept(
                        "s060_buffer_stems", "s060_brake_shoes", "s060_cab_light_bulb", "s060_gauge_lubricator_glass").ToList();
                case LocoS282730AConfig _:
                    result = new List<GameObject>();
                    result.AddRange(transform.Find("LocoS282A_Body/Static_LOD0").AllChildGOsExcept(
                        "s282_buffer_stems", "s282_brake_shoes", "s282_glass", "s282_valve_gear_light", "s282_gauge_lubricator_glass"));
                    result.AddRange(transform.Find("LocoS282A_Body/Static_LOD1").AllChildGOsExcept("s282_buffer_stems_LOD1"));
                    result.AddRange(transform.Find("LocoS282A_Body/Static_LOD2").AllChildGOs());
                    result.AddRange(transform.Find("LocoS282A_Body/Static_LOD3").AllChildGOs());
                    return result;
                case LocoS282730BConfig _:
                    result = new List<GameObject>();
                    result.AddRange(transform.Find("LocoS282B_Body/LOD0").AllChildGOsExcept("s282_tender_buffer_stems"));
                    result.AddRange(transform.Find("LocoS282B_Body/LOD1").AllChildGOsExcept("s282_tender_buffer_stems_LOD1"));
                    result.AddRange(transform.Find("LocoS282B_Body/LOD2").AllChildGOs());
                    result.AddRange(transform.Find("LocoS282B_Body/LOD3").AllChildGOs());
                    return result;

                case LocoBE2260Config _:
                    return new List<GameObject>
                    {
                        transform.Find("LocoMicroshunter_Body/microshunter_body_LOD0").gameObject,
                        transform.Find("LocoMicroshunter_Body/microshunter_body_LOD1").gameObject,
                        transform.Find("LocoMicroshunter_Body/microshunter_body_LOD2").gameObject,
                        transform.Find("LocoMicroshunter_Body/microshunter_body_LOD3").gameObject
                    };

                case LocoDE6860SlugConfig _:
                    return transform.Find("LocoDE6Slug_Body").AllChildGOsExcept("de6_slug_buffer_stems").ToList();
                case LocoHandcarConfig _:
                    return transform.Find("LocoHandcar_Body").AllChildGOsExcept("crank mechanism").ToList();

                default:
                    break;
            }

            foreach (Transform t in transform)
            {
                // Skip buffers just in case.
                if (t.name == "[buffers]") continue;

                // First LODGroup in top level children should be the body.
                if (t.TryGetComponent<LODGroup>(out var lod))
                {
                    return new List<GameObject> { lod.gameObject };
                }
            }

            // Don't worry about it if it's a custom car config,
            // it won't be used anyways.
            if (Config is CustomCarConfig)
            {
                return null!;
            }

            CarChangerMod.Error("Could not find original body!");
            return null!;
        }

        private List<GameObject> GetOriginalInteriorLod()
        {
            List<GameObject> result;

            // Ditto, for the interior LOD.
            switch (Config)
            {
                case PassengerConfig _:
                    return new List<GameObject>
                    {
                        transform.Find("CarPassenger/CarPassengerInterior_LOD0").gameObject,
                        transform.Find("CarPassenger/CarPassengerInterior_LOD1").gameObject,
                        transform.Find("CarPassenger/CarPassengerInterior_LOD2").gameObject
                    };
                case LocoDE2480Config de2:
                    result = new List<GameObject>
                    {
                        transform.Find("[interior LOD]/InteriorLOD/cab_LOD1").gameObject,
                        transform.Find("[interior LOD]/InteriorLOD/cab_LOD2").gameObject
                    };

                    if (de2.HideControlDeck)
                    {
                        result.Add(transform.Find("[interior LOD]/InteriorLOD/deck_LOD1").gameObject);
                    }

                    return result;
                default:
                    break;
            }

            var lod = transform.Find("[interior LOD]");

            // If there's no interior lod object, return an emtpy list.
            if (lod == null) return new List<GameObject>();

            // By default, return all children of that object.
            // Exclude stuff added by changes.
            return lod.AllChildGOs().Where(x => !x.name.EndsWith(InteriorLODNamingEnd)).ToList();
        }

        #region Changing

        private void ChangeBody(GameObject? body, bool hideOriginal)
        {
            if (body != null)
            {
                _body = Instantiate(body, transform);
                ComponentProcessor.ProcessComponents(_body, MatHolder);
            }

            if (hideOriginal)
            {
                foreach (var item in _originalBody)
                {
                    item.SetActive(false);
                }

                _bodyHidden = true;
            }
        }

        private void ChangeInteriorLod(GameObject? interior, bool hideOriginal)
        {
            if (interior != null)
            {
                _interiorLod = Instantiate(interior, transform.Find("[interior LOD]") ?? transform);
                ComponentProcessor.ProcessComponents(_interiorLod, MatHolder);

                _interiorLod.name = $"{_interiorLod.name}{InteriorLODNamingEnd}";
            }

            if (hideOriginal)
            {
                foreach (var item in _originalInteriorLod)
                {
                    item.SetActive(false);
                }

                _interiorLodHidden = true;
            }
        }

        private void ChangeInterior(IInteriorChanger changer)
        {
            _interior = changer;
            _interior.Apply(TrainCar.loadedInterior);

            TrainCar.InteriorLoaded += _interior.Apply;
        }

        private void ChangeInteractables(IInteractablesChanger changer)
        {
            _interactables = changer;
            _interactables.Apply(TrainCar.loadedExternalInteractables);

            TrainCar.ExternalInteractableLoaded += _interactables.Apply;
        }

        #endregion

        #region Resets

        private void ResetBogies()
        {
            _bogieChanger?.Reset();
        }

        private void ResetBody()
        {
            Helpers.DestroyIfNotNull(_body);

            if (_bodyHidden)
            {
                foreach (var item in _originalBody)
                {
                    item.SetActive(true);
                }
            }

            _body = null!;
            _bodyHidden = false;
        }

        private void ResetInteriorLod()
        {
            Helpers.DestroyIfNotNull(_interiorLod);

            if (_interiorLodHidden)
            {
                foreach (var item in _originalInteriorLod)
                {
                    item.SetActive(true);
                }
            }

            _interiorLod = null!;
            _interiorLodHidden = false;
        }

        private void ResetInterior()
        {
            if (_interior == null) return;

            TrainCar.InteriorLoaded -= _interior.Apply;
            _interior.Unapply(TrainCar.loadedInterior);
            _interior = null;
        }

        private void ResetInteractables()
        {
            if (_interactables == null) return;

            TrainCar.ExternalInteractableLoaded -= _interactables.Apply;
            _interactables.Unapply(TrainCar.loadedExternalInteractables);
            _interactables = null;
        }

        private void ResetHeadlights()
        {
            _frontHeadlights?.Unapply();
            _rearHeadlights?.Unapply();
            _frontHeadlights = null;
            _rearHeadlights = null;
        }

        private void ResetColliders()
        {
            if (_colliderHolder == null) return;

            _colliderHolder.Unapply();
            _colliderHolder = null;
        }

        #endregion

        private void LogChange()
        {
            CarChangerMod.LogVerbose($"Applying change {Config!.ModificationId} to [{TrainCar.ID}|{TrainCar.carLivery.id}]");
        }

        internal void ForceApplyChange(string? reason, int delay = 1)
        {
            if (!string.IsNullOrEmpty(reason))
            {
                CarChangerMod.LogVerbose($"Forcing change application due to: {reason}");
            }

            StartCoroutine(ApplyChangeRoutine(delay));
        }

        internal void ReapplyForPaint(TrainCarPaint paint)
        {
            ForceApplyChange("repaint");
        }

        public static bool CanApplyChange(TrainCar car, ModelConfig config)
        {
            foreach (var item in car.GetComponents<AppliedChange>())
            {
                if (item.Config == null) continue;

                if (!ModelConfig.CanCombine(config, item.Config))
                {
                    return false;
                }
            }

            return true;
        }

        public static AppliedChange AddChange(TrainCar car, ModelConfig config)
        {
            var change = car.gameObject.AddComponent<AppliedChange>();
            change.Config = config;
            return change;
        }

        public static AppliedChange OverrideChange(TrainCar car, ModelConfig config)
        {
            foreach (var item in car.GetComponents<AppliedChange>())
            {
                // Destroy all changes that are incompatible.
                if (item.Config == null || !ModelConfig.CanCombine(config, item.Config))
                {
                    Destroy(item);
                }
            }

            // Add the intended config and apply.
            return AddChange(car, config);
        }
    }
}
