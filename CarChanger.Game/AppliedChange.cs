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
        private GameObject DefaultBogie => TrainCar.carLivery.prefab.GetComponentInChildren<Bogie>().transform.GetChild(0).gameObject;

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

        private void Awake()
        {
            TrainCar = TrainCar.Resolve(gameObject);
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

        public static void ChangeBogies(TrainCar car, GameObject frontBogie, GameObject rearBogie, float? radius)
        {
            // Store them for quick access.
            var bogies = car.Bogies;
            var bogieF = bogies[1];
            var bogieR = bogies[0];

            // Invalidate cached axles.
            Helpers.InvalidateBogieCache(bogieF);
            Helpers.InvalidateBogieCache(bogieR);

            if (frontBogie)
            {
                // Yeet the original bogie by unparenting and deleting.
                // If it is not unparented the game will still find it.
                var orig = bogieF.transform.GetChild(0);
                orig.parent = null;
                Destroy(orig.gameObject);

                var bogie = Instantiate(frontBogie, bogieF.transform);
                bogie.name = Constants.BogieName;
            }

            if (rearBogie)
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

                    foreach (var item in powered)
                    {
                        var axleAnchors = sparks.OrderBy(x => (item.wheelTransform.position - x.position).sqrMagnitude).Take(2)
                            .OrderBy(x => x.position.x).ToArray();

                        anchors.Add(new WheelslipSparksController.WheelSparksDefinition
                        {
                            poweredWheel = item,
                            sparksLeftAnchor = axleAnchors[0],
                            sparksRightAnchor = axleAnchors[1]
                        });
                    }

                    wheelSlip.wheelSparks = anchors.ToArray();
                }
            }
        }

        private void ChangeBody(GameObject body, bool hideOriginal)
        {
            // Instantiate the new modification.
            if (body)
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

        #region Application

        private void ReturnToDefault()
        {
            // No need to output anything if the car isn't in the world anymore.
            if (TrainCar.logicCar != null)
            {
                if (Config != null)
                {
                    CarChangerMod.Log($"Removing change {Config.ModificationId} from [{TrainCar.ID}|{TrainCar.carLivery.id}]");
                }
                else
                {
                    CarChangerMod.Log($"Returning to default [{TrainCar.ID}|{TrainCar.carLivery.id}]");
                }
            }

            ResetBogies();
            ResetBody();
            ResetHeadlights();
            ResetInterior();
            ResetInteractables();

            if (_explosionHandler != null)
            {
                Destroy(_explosionHandler.gameObject);
            }

            Config?.Unapplied(TrainCar.gameObject);
            _changeApplied = false;
        }

        private void ApplyWagon(WagonConfig config)
        {
            CarChangerMod.Log($"Applying change {config.ModificationId} to [{TrainCar.ID}|{TrainCar.carLivery.id}]");

            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody.GetComponentInChildren<Renderer>().material
            };

            if (config.UseCustomBogies)
            {
                _bogiesChanged = true;
                ChangeBogies(TrainCar, config.FrontBogie, config.RearBogie, config.WheelRadius);
            }

            _bodyHidden = config.HideOriginalBody;
            ChangeBody(config.BodyPrefab, config.HideOriginalBody);
        }

        private void ApplyGroup(ModificationGroupConfig config)
        {
            foreach (var item in config.ModificationsToActivate)
            {
                gameObject.AddComponent<AppliedChange>().Config = item;
            }

            Destroy(this);
        }

        private void ApplyDE6(LocoDE6Config config)
        {
            CarChangerMod.Log($"Applying change {config.ModificationId} to [{TrainCar.ID}|{TrainCar.carLivery.id}]");
            
            MatHolder = new MaterialHolder(TrainCar)
            {
                Body = _originalBody.GetComponentInChildren<Renderer>().material,
                Interior = TrainCar.transform.Find(
                    "[interior LOD]/LocoDE6_InteriorLOD/cab_LOD1").GetComponent<Renderer>().material,
                Glass = TrainCar.transform.Find(
                    "LocoDE6_Body/windows/window_01").GetComponent<Renderer>().material,
                BodyExploded = TrainCar.carLivery.explodedExternalInteractablesPrefab.transform.Find(
                    "DoorsWindows/DoorR/C_DoorR/cab_door01a").GetComponent<Renderer>().material,
                InteriorExploded = TrainCar.carLivery.explodedInteriorPrefab.transform.Find(
                    "Cab").GetComponent<Renderer>().material,
                GlassBroken = TrainCar.transform.Find(
                    "LocoDE6_Body/broken_windows").GetComponent<Renderer>().material,
            };

            if (config.UseCustomBogies)
            {
                _bogiesChanged = true;
                _bogiesPowered = true;

                var wheelStates = TrainCar.Bogies.SelectMany(x => x.Axles)
                    .Select(x => x.transform)
                    .OrderBy(x => TrainCar.transform.InverseTransformPoint(x.position).z)
                    .Select(x => x.GetComponent<PoweredWheel>().state);

                ChangeBogies(TrainCar, config.FrontBogie, config.RearBogie, config.WheelRadius);
                MakeBogiesPowered(TrainCar, wheelStates.ToArray(), config.WheelRadius);
            }

            _bodyHidden = config.HideOriginalBody;
            ChangeBody(config.BodyPrefab, config.HideOriginalBody);

            if (config.UseCustomFrontHeadlights)
            {
                _frontHeadlights = new LocoDE6HeadlightChanger(config, TrainCar, HeadlightDirection.Front);
                _frontHeadlights.Apply();
            }

            if (config.UseCustomRearHeadlights)
            {
                _rearHeadlights = new LocoDE6HeadlightChanger(config, TrainCar, HeadlightDirection.Rear);
                _rearHeadlights.Apply();
            }

            ChangeInterior(new LocoDE6InteriorChanger(config, MatHolder));
        }

        private void ApplyCustomCar(CustomCarConfig config)
        {
            CarChangerMod.Log($"Applying change {config.ModificationId} to [{TrainCar.ID}|{TrainCar.carLivery.id}]");

            MatHolder = new MaterialHolder(TrainCar);
            ChangeBody(config.BodyPrefab, false);
        }

        #endregion
    }
}
