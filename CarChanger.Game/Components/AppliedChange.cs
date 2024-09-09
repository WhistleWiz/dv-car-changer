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

namespace CarChanger.Game.Components
{
    internal partial class AppliedChange : MonoBehaviour
    {
        public TrainCar TrainCar = null!;
        public ModelConfig? Config = null;
        public event Action<AppliedChange>? OnApply = null;
        public MaterialHolder MatHolder = null!;

        private bool _changeApplied = false;
        private List<GameObject> _originalBody = new List<GameObject>();
        private List<GameObject> _originalInteriorLod = new List<GameObject>();
        private GameObject _body = null!;
        private GameObject _interiorLod = null!;
        private bool _bogiesChanged = false;
        private bool _bogiesPowered = false;
        private bool _bodyHidden = false;
        private bool _interiorLodHidden = false;
        private HeadlightChanger? _frontHeadlights = null;
        private HeadlightChanger? _rearHeadlights = null;
        private IInteriorChanger? _interior = null;
        private IInteractablesChanger? _interactables = null;
        private ExplosionModelHandler? _explosionHandler = null;
        private ColliderHolder? _colliderHolder = null;

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
                case LocoDE6Config de6:
                    ApplyDE6(de6);
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
                case LocoDE6SlugConfig de6Slug:
                    ApplyDE6Slug(de6Slug);
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
                case LocoDE6Config _:
                    return new List<GameObject>
                    {
                        transform.Find("LocoDE6_Body/Body").gameObject
                    };
                case LocoS282730AConfig _:
                    result = new List<GameObject>();
                    result.AddRange(transform.Find("LocoS282A_Body/Static_LOD0").AllChildGOsExcept("s282_buffer_stems", "s282_brake_shoes"));
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
                case LocoDE6SlugConfig _:
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
            return lod.AllChildGOs().ToList();
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
            // This method only works on unpowered vehicles, since powered use a different thing.
            // For those a separate method is used to update.
            var wheelRotation = car.GetComponentInChildren<WheelRotationViaCode>();

            if (wheelRotation != null)
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

                var bogies = TrainCar.carLivery.prefab.GetComponentsInChildren<Bogie>()
                    .OrderBy(x => x.transform.position.z)
                    .Select(x => x.transform.GetChild(0).gameObject);
                ChangeBogies(TrainCar, bogies.ElementAt(1), bogies.ElementAt(0), TrainCar.carLivery.parentType.wheelRadius);

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

        private void ResetHeadlights()
        {
            _frontHeadlights?.Unapply();
            _rearHeadlights?.Unapply();
            _frontHeadlights = null;
            _rearHeadlights = null;
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

        private void LogChange()
        {
            CarChangerMod.Log($"Applying change {Config!.ModificationId} to [{TrainCar.ID}|{TrainCar.carLivery.id}]");
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
    }
}
