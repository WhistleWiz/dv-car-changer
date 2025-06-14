using CarChanger.Common;
using CarChanger.Common.Components;
using CarChanger.Game.Components;
using DV.Simulation.Brake;
using DV.Wheels;
using LocoSim.Implementations.Wheels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game
{
    internal class BogieChanger
    {
        private MaterialHolder _matHolder;
        private Transform _originalF;
        private Transform _originalR;
        private GameObject? _newF;
        private GameObject? _newR;
        private bool _powered;
        private float _radius;
        private ExplosionModelHandler? _explosionHandler;

        private TrainCar Car => _matHolder.Car;

        public BogieChanger(MaterialHolder holder, GameObject? frontBogie, GameObject? rearBogie, float? radius, bool powered = false)
        {
            _matHolder = holder;
            _powered = powered;
            _radius = radius ?? Car.carLivery.parentType.wheelRadius;

            // Store them for quick access.
            var bogies = Car.Bogies;
            var bogieF = bogies[1];
            var bogieR = bogies[0];

            // Invalidate cached axles.
            Helpers.InvalidateBogieCache(bogieF);
            Helpers.InvalidateBogieCache(bogieR);

            _originalF = bogieF.transform.GetChild(0);
            _originalR = bogieR.transform.GetChild(0);

            if (frontBogie != null)
            {
                _newF = Object.Instantiate(frontBogie, bogieF.transform);
                _newF.name = Constants.BogieName;

                _originalF.name = "[replaced]";
                _originalF.gameObject.SetActive(false);

                ComponentProcessor.ProcessComponentsMinimal(_newF, _matHolder);
                ModIntegrations.Gauge.RegaugeBogie(bogieF);
            }

            if (rearBogie != null)
            {
                _newR = Object.Instantiate(rearBogie, bogieR.transform);
                _newR.name = Constants.BogieName;

                _originalR.name = "[replaced]";
                _originalR.gameObject.SetActive(false);

                ComponentProcessor.ProcessComponentsMinimal(_newR, _matHolder);
                ModIntegrations.Gauge.RegaugeBogie(bogieR);
            }

            CommonProcedure(bogieF, bogieR, _radius);

            if (_powered)
            {
                CommonPoweredProcedure(_radius);
            }

            var disableGos = new List<DisableGameObjectOnExplosion>();
            var goSwaps = new List<SwapGameObjectOnExplosion>();
            var matSwaps = new List<SwapMaterialOnExplosion>();

            if (_newF != null)
            {
                CarChangerExplosionManager.GetEntries(_newF, out var disableGosTemp, out var goSwapsTemp, out var matSwapsTemp);
                disableGos.AddRange(disableGosTemp);
                goSwaps.AddRange(goSwapsTemp);
                matSwaps.AddRange(matSwapsTemp);
            }

            if (_newR != null)
            {
                CarChangerExplosionManager.GetEntries(_newR, out var disableGosTemp, out var goSwapsTemp, out var matSwapsTemp);
                disableGos.AddRange(disableGosTemp);
                goSwaps.AddRange(goSwapsTemp);
                matSwaps.AddRange(matSwapsTemp);
            }

            _explosionHandler = CarChangerExplosionManager.PrepareExplosionHandlerFromEntries(holder, disableGos, goSwaps, matSwaps);
        }

        public void Reset()
        {
            var bogies = Car.Bogies;
            var bogieF = bogies[1];
            var bogieR = bogies[0];

            _originalF.name = Constants.BogieName;
            _originalR.name = Constants.BogieName;
            _originalF.gameObject.SetActive(true);
            _originalR.gameObject.SetActive(true);

            if (_newF != null)
            {
                _newF.name = "[destroyed]";
                Object.Destroy(_newF);
            }
            if (_newR != null)
            {
                _newR.name = "[destroyed]";
                Object.Destroy(_newR);
            }

            Helpers.InvalidateBogieCache(bogieF);
            Helpers.InvalidateBogieCache(bogieR);

            CommonProcedure(bogieF, bogieR, Car.carLivery.parentType.wheelRadius);

            if (_powered)
            {
                CommonPoweredProcedure(Car.carLivery.parentType.wheelRadius);
            }

            Helpers.DestroyGameObjectIfNotNull(_explosionHandler);
        }

        private void CommonProcedure(Bogie bogieF, Bogie bogieR, float radius)
        {
            // Get brake renderers. Includes those at the default path, and custom ones within the bogies
            // as defined by the components.
            // Needs to check if it exist as some cars may not have it (caboose).
            if (Car.gameObject.TryGetComponentInChildren(out BrakesOverheatingController overheat))
            {
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

                overheat.brakeRenderers = brakes.ToArray();
            }

            // Wheel sparks too.
            if (Car.gameObject.TryGetComponentInChildren(out WheelSlideSparksController sparksController))
            {
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

                sparksController.sparkAnchors = sparks.ToArray();
            }

            // Finally, update the wheel rotation.
            // This method only works on unpowered vehicles, since powered use a different thing.
            // For those a separate method is used to update.
            if (Car.gameObject.TryGetComponentInChildren(out WheelRotationViaCode wheelRotation))
            {
                wheelRotation.wheelRadius = radius;

                Transform[] transformsToRotate = Car.Bogies.SelectMany((Bogie b) => b.Axles.Select((Bogie.AxleInfo a) => a.transform)).ToArray();
                wheelRotation.transformsToRotate = transformsToRotate;
            }
        }

        private void CommonPoweredProcedure(float radius)
        {
            var wheelRotation = Car.GetComponentInChildren<PoweredWheelRotationViaCode>();
            var manager = Car.GetComponentInChildren<PoweredWheelsManager>();

            // Can't make them powered in this case.
            if (!wheelRotation || !manager) return;

            var wheelStates = manager.poweredWheels
                .OrderBy(x => Car.transform.InverseTransformPoint(x.transform.position).z)
                .Select(x => x.GetComponent<PoweredWheel>().state).ToArray();

            wheelRotation.wheelRadius = radius;

            // Get all axles, ordered by position in relation to the car for consistency.
            var axles = Car.Bogies.SelectMany(x => x.Axles).Select(x => x.transform).OrderBy(x => Car.transform.InverseTransformPoint(x.position).z);
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
                    Object.Destroy(axle);
                    continue;
                }
            }

            manager.poweredWheels = powered.ToArray();

            if (Car.gameObject.TryGetComponentInChildren(out WheelslipSparksController wheelSlip))
            {
                var sparks = Car.GetComponentInChildren<WheelSlideSparksController>().sparkAnchors;
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
}
