using CarChanger.Common;
using CarChanger.Common.Components;
using CarChanger.Common.Configs;
using CarChanger.Game.HeadlightChanges;
using CarChanger.Game.InteriorChanges;
using DV.Simulation.Brake;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using DV.Wheels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game
{
    internal class AppliedChange : MonoBehaviour
    {
        public class MaterialHolder
        {
            public TrainCar Car;
            public Material Body = null!;
            public Material Interior = null!;
            public Material Glass = null!;

            public MaterialHolder(TrainCar car)
            {
                Car = car;
            }

            public Material GetFromPath(UseBodyMaterial path)
            {
                var renderer = Car.transform.Find(path.MaterialObjectPath).GetComponent<Renderer>();

                if (renderer == null)
                {
                    return null!;
                }

                else return renderer.material;
            }
        }

        private static GameObject? s_defaultBogie = null;

        private static GameObject DefaultBogie => Helpers.GetCached(ref s_defaultBogie,
            () => TrainCarType.FlatbedEmpty.ToV2().prefab.transform.Find($"{Constants.BogieF}/{Constants.BogieName}").gameObject);

        public TrainCar TrainCar = null!;
        public ModelConfig? Config = null;
        public event Action<AppliedChange>? OnApply = null;
        public MaterialHolder MatHolder = null!;

        private GameObject _originalBody = null!;
        private GameObject _body = null!;
        private bool _bogiesChanged = false;
        private bool _bodyHidden = false;
        private IHeadlightChanger? _frontHeadlights = null;
        private IHeadlightChanger? _rearHeadlights = null;
        private IInteriorChanger? _interior = null;

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
            if (!TrainCar || !gameObject || TrainCar.logicCar == null)
            {
                return;
            }

            ReturnToDefault();
        }

        public void ApplyChange()
        {
            if (Config == null)
            {
                CarChangerMod.Warning($"No config on AppliedChange '{name}'!");
                return;
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
                default:
                    ReturnToDefault();
                    return;
            }

            Config.Applied(TrainCar.gameObject);
            OnApply?.Invoke(this);
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
                if (radius.HasValue)
                {
                    wheelRotation.wheelRadius = radius.Value;
                }

                Transform[] transformsToRotate = bogies.SelectMany((Bogie b) => b.Axles.Select((Bogie.AxleInfo a) => a.transform)).ToArray();
                wheelRotation.transformsToRotate = transformsToRotate;
            }
        }

        private void ChangeBody(GameObject body, bool hideOriginal)
        {
            // Destroy the modification applied by this component.
            if (_body != null)
            {
                Destroy(_body);
            }

            // Instantiate the new modification.
            if (body)
            {
                _body = Instantiate(body, transform);
                ProcessComponents(_body, MatHolder);
            }

            // If the original body is missing, end here.
            if (!_originalBody)
            {
                return;
            }

            if (_bodyHidden)
            {
                _originalBody.SetActive(!hideOriginal);
            }
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
            _interior?.Unapply();
            _interior = null;
        }

        public static void ProcessComponents(GameObject gameObject, MaterialHolder holder)
        {
            foreach (var item in gameObject.GetComponentsInChildren<UseBodyMaterial>())
            {
                switch (item.Material)
                {
                    case SourceMaterial.BodyDefault:
                        item.GetComponent<Renderer>().material = holder.Body;
                        break;
                    case SourceMaterial.InteriorDefault:
                        item.GetComponent<Renderer>().material = holder.Interior;
                        break;
                    case SourceMaterial.Glass:
                        item.GetComponent<Renderer>().material = holder.Glass;
                        break;
                    case SourceMaterial.DE6Engine:
                        break;
                    case SourceMaterial.Custom:
                        item.GetComponent<Renderer>().material = holder.GetFromPath(item);
                        break;
                    default:
                        break;
                }
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
            if (Config != null)
            {
                CarChangerMod.Log($"Removing change {Config.ModificationId} from [{TrainCar.ID}|{TrainCar.carLivery.id}]");
            }
            else
            {
                CarChangerMod.Log($"Returning to default [{TrainCar.ID}|{TrainCar.carLivery.id}]");
            }

            ChangeBogies(TrainCar, _bogiesChanged ? DefaultBogie : null!, _bogiesChanged ? DefaultBogie : null!,
                _bogiesChanged ? TrainCar.carLivery.parentType.wheelRadius : (float?)null);
            ChangeBody(null!, false);
            ResetHeadlights();
            ResetInterior();

            Config?.Unapplied(TrainCar.gameObject);
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
                Interior = TrainCar.transform.Find("[interior LOD]/LocoDE6_InteriorLOD/cab_LOD1").GetComponent<Renderer>().material,
                Glass = TrainCar.transform.Find("LocoDE6_Body/windows/window_01").GetComponent<Renderer>().material
            };

            if (config.UseCustomBogies)
            {
                _bogiesChanged = true;
                ChangeBogies(TrainCar, config.FrontBogie, config.RearBogie, config.WheelRadius);
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

            _interior = new LocoDE6InteriorChanger(config, MatHolder);

            if (TrainCar.loadedInterior)
            {
                _interior.Apply(TrainCar.loadedInterior);
            }
        }

        #endregion
    }
}
