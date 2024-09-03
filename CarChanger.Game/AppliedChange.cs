using CarChanger.Common;
using CarChanger.Common.Components;
using CarChanger.Common.Configs;
using CarChanger.Game.HeadlightChanges;
using CarChanger.Game.InteractablesChanges;
using CarChanger.Game.InteriorChanges;
using DV.Simulation.Brake;
using DV.Wheels;
using LocoSim.Implementations.Wheels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game
{
    internal partial class AppliedChange : MonoBehaviour
    {

        public TrainCar TrainCar = null!;
        public ModelConfig? Config = null;
        public event Action<AppliedChange>? OnApply = null;
        public MaterialHolder MatHolder = null!;

        private bool _changeApplied = false;
        private GameObject _originalBody = null!;
        private GameObject _body = null!;
        private bool _bogiesChanged = false;
        private bool _bogiesPowered = false;
        private bool _bodyHidden = false;
        private IHeadlightChanger? _frontHeadlights = null;
        private IHeadlightChanger? _rearHeadlights = null;
        private IInteriorChanger? _interior = null;
        private IInteractablesChanger? _interactables = null;
        private ExplosionModelHandler? _explosionHandler = null;
        private ColliderHolder? _colliderHolder = null;

        private GameObject DefaultBogie => TrainCar.carLivery.prefab.GetComponentInChildren<Bogie>().transform.GetChild(0).gameObject;

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
        }

        private void ApplyChange()
        {
            if (Config == null)
            {
                CarChangerMod.Warning($"No config on AppliedChange '{name}'!");
                return;
            }

            if (_changeApplied)
            {
                ReturnToDefault();
            }

            _originalBody = GetOriginalBody();

            switch (Config)
            {
                case WagonConfig wagon:
                    ApplyWagon(wagon);
                    break;
                case ModificationGroupConfig group:
                    ApplyGroup(group);
                    break;
                case LocoDE6Config de6:
                    ApplyDE6(de6);
                    break;
                case CustomCarConfig custom:
                    ApplyCustomCar(custom);
                    break;
                default:
                    return;
            }

            // If the applied change resulted in a body GameObject, see if it explodes.
            if (_body != null)
            {
                _explosionHandler = CarChangerExplosionManager.PrepareExplosionHandler(_body, MatHolder);
            }

            _colliderHolder?.Apply();

            Config.Applied(TrainCar.gameObject);
            OnApply?.Invoke(this);

            _changeApplied = true;
        }

        private GameObject GetOriginalBody()
        {
            // Get the body object at the path.
            // Locos have a special path to ensure everything works fine.
            switch (Config)
            {
                case LocoDE6Config _:
                    return transform.Find("LocoDE6_Body/Body").gameObject;
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
                    return lod.gameObject;
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

        private PoweredWheel.State[] GetCurrentPoweredWheelStates()
        {
            // Pick the axles, order them for consistency, then get the state
            // from the PoweredWheel component attached to them.
            return TrainCar.Bogies.SelectMany(x => x.Axles)
                .Select(x => x.transform)
                .OrderBy(x => TrainCar.transform.InverseTransformPoint(x.position).z)
                .Select(x => x.GetComponent<PoweredWheel>().state).ToArray();
        }

        #region Changing

        public static void ChangeBogies(TrainCar car, GameObject? frontBogie, GameObject? rearBogie, float? radius)
        {
            // Store them for quick access.
            var bogies = car.Bogies;
            var bogieF = bogies[1];
            var bogieR = bogies[0];

            // Invalidate cached axles.
            Helpers.InvalidateBogieCache(bogieF);
            Helpers.InvalidateBogieCache(bogieR);

            if (frontBogie != null)
            {
                // Yeet the original bogie by unparenting and deleting.
                // If it is not unparented the game will still find it.
                var orig = bogieF.transform.GetChild(0);
                orig.parent = null;
                Destroy(orig.gameObject);

                var bogie = Instantiate(frontBogie, bogieF.transform);
                bogie.name = Constants.BogieName;
            }

            if (rearBogie != null)
            {
                // Do the same with the rear bogie.
                var orig = bogieR.transform.GetChild(0);
                orig.parent = null;
                Destroy(orig.gameObject);

                var bogie = Instantiate(rearBogie, bogieR.transform);
                bogie.name = Constants.BogieName;
            }

            // Get brake renderers. Includes those at the default path, and custom ones within the bogies
            // as defined by the components.
            List<Renderer> brakes = new List<Renderer>();

            var padsF = bogieF.transform.Find(Constants.BogieBrakePadsPath);
            var padsR = bogieR.transform.Find(Constants.BogieBrakePadsPath);

            if (padsF)
            {
                brakes.AddRange(padsF.GetComponentsInChildren<Renderer>());
            }
            if (padsR)
            {
                brakes.AddRange(padsR.GetComponentsInChildren<Renderer>());
            }

            brakes.AddRange(bogieF.GetComponentsInChildren<ExtraBrakeRenderer>().Select(x => x.GetRenderer()));
            brakes.AddRange(bogieR.GetComponentsInChildren<ExtraBrakeRenderer>().Select(x => x.GetRenderer()));

            car.GetComponentInChildren<BrakesOverheatingController>().brakeRenderers = brakes.ToArray();

            // Wheel sparks too.
            List<Transform> sparks = new List<Transform>();

            var contactsF = bogieF.transform.Find(Constants.BogieContactPointsPath);
            var contactsR = bogieR.transform.Find(Constants.BogieContactPointsPath);

            if (contactsF)
            {
                // Get children but not self.
                foreach (Transform t in contactsF)
                {
                    sparks.Add(t);
                }
            }
            if (contactsR)
            {
                foreach (Transform t in contactsR)
                {
                    sparks.Add(t);
                }
            }

            car.GetComponentInChildren<WheelSlideSparksController>().sparkAnchors = sparks.ToArray();

            // Finally, update the wheel rotation.
            // This method only works on unpowered vehicles, since powered use a different location for this
            // component, or something else only.
            // For those a separate method is used to update.
            if (car.TryGetComponent<WheelRotationViaCode>(out var wheelRotation))
            {
                wheelRotation.wheelRadius = radius ?? wheelRotation.wheelRadius;

                Transform[] transformsToRotate = bogies.SelectMany((Bogie b) => b.Axles.Select((Bogie.AxleInfo a) => a.transform)).ToArray();
                wheelRotation.transformsToRotate = transformsToRotate;
            }
        }

        public static void MakeBogiesPowered(TrainCar car, IList<PoweredWheel.State> wheelStates, float? radius)
        {
            var wheelRotation = car.GetComponentInChildren<PoweredWheelRotationViaCode>();
            var manager = car.GetComponentInChildren<PoweredWheelsManager>();

            if (wheelRotation && manager)
            {
                wheelRotation.wheelRadius = radius ?? wheelRotation.wheelRadius;

                // Get all axles, ordered by position in relation to the car for consistency.
                var axles = car.Bogies.SelectMany(x => x.Axles).Select(x => x.transform).OrderBy(x => car.transform.InverseTransformPoint(x.position).z);
                int i = 0;
                List<PoweredWheel> powered = new List<PoweredWheel>();

                foreach (var item in axles)
                {
                    if (item.TryGetComponent<PoweredWheel>(out var wheel))
                    {
                        wheel.state = wheelStates[i++];
                        powered.Add(wheel);
                        continue;
                    }

                    if (item.TryGetComponent<PoweredAxle>(out var axle))
                    {
                        wheel = item.gameObject.AddComponent<PoweredWheel>();
                        wheel.wheelTransform = item;
                        wheel.localRotationAxis = axle.Axis;
                        wheel.state = wheelStates[i++];
                        powered.Add(wheel);
                        Destroy(axle);
                        continue;
                    }
                }

                manager.poweredWheels = powered.ToArray();

                var wheelSlip = car.GetComponentInChildren<WheelslipSparksController>();

                if (wheelSlip)
                {
                    var sparks = car.GetComponentInChildren<WheelSlideSparksController>().sparkAnchors;
                    var anchors = new List<WheelslipSparksController.WheelSparksDefinition>();

                    if (sparks != null && sparks.Length > 1)
                    {
                        foreach (var item in powered)
                        {
                            // Find the 2 closest anchors to the axle. If set up properly, they are the right ones.
                            // Order them by their X coordinate for left/right.
                            var axleAnchors = sparks.OrderBy(x => (item.wheelTransform.position - x.position).sqrMagnitude)
                                .Take(2).OrderBy(x => x.position.x);

                            var definition = new WheelslipSparksController.WheelSparksDefinition
                            {
                                poweredWheel = item,
                                sparksLeftAnchor = axleAnchors.ElementAt(0),
                                sparksRightAnchor = axleAnchors.ElementAt(1)
                            };

                            definition.Init();
                            anchors.Add(definition);
                        }
                    }
                    else
                    {
                        CarChangerMod.Error("Wheel contact points were not setup correctly! Wheel slip sparks will not work.");
                    }

                    wheelSlip.wheelSparks = anchors.ToArray();
                }
            }
        }

        private void ChangeBody(GameObject? body, bool hideOriginal)
        {
            // Instantiate the new modification.
            if (body != null)
            {
                _body = Instantiate(body, transform);
                ComponentProcessor.ProcessComponents(_body, MatHolder);
            }

            // If the original body is missing, end here.
            if (!_originalBody) return;

            if (hideOriginal)
            {
                _originalBody.SetActive(false);
                _bodyHidden = true;
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
            if (_bogiesChanged)
            {
                // States must be extracted before changing the bogies, and reapplied after
                // the new bogies are instanced. Thus the mess.
                PoweredWheel.State[] wheelStates = null!;

                if (_bogiesPowered)
                {
                    wheelStates = TrainCar.Bogies.SelectMany(x => x.Axles)
                        .Select(x => x.transform)
                        .OrderBy(x => TrainCar.transform.InverseTransformPoint(x.position).z)
                        .Select(x => x.GetComponent<PoweredWheel>().state).ToArray();
                }

                ChangeBogies(TrainCar, DefaultBogie, DefaultBogie, TrainCar.carLivery.parentType.wheelRadius);

                if (_bogiesPowered)
                {
                    MakeBogiesPowered(TrainCar, wheelStates, TrainCar.carLivery.parentType.wheelRadius);
                }

                _bogiesChanged = false;
                _bogiesPowered = false;
            }
        }

        private void ResetBody()
        {
            if (_body)
            {
                Destroy(_body);
            }

            if (_bodyHidden && _originalBody)
            {
                _originalBody.SetActive(true);
            }

            _body = null!;
            _bodyHidden = false;
        }

        private void ResetHeadlights()
        {
            _frontHeadlights?.Unapply();
            _rearHeadlights?.Unapply();
            _frontHeadlights = null;
            _rearHeadlights = null;
        }

        private void ResetInterior()
        {
            if (_interior != null)
            {
                TrainCar.InteriorLoaded -= _interior.Apply;
                _interior.Unapply(TrainCar.loadedInterior);
                _interior = null;
            }
        }

        private void ResetInteractables()
        {
            if (_interactables != null)
            {
                TrainCar.ExternalInteractableLoaded -= _interactables.Apply;
                _interactables.Unapply(TrainCar.loadedExternalInteractables);
                _interactables = null;
            }
        }

        private void ResetColliders()
        {
            if (_colliderHolder != null)
            {
                _colliderHolder.Unapply();
                _colliderHolder = null;
            }
        }

        #endregion

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
            var change = car.gameObject.AddComponent<AppliedChange>();
            change.Config = config;
            return change;
        }
    }
}
