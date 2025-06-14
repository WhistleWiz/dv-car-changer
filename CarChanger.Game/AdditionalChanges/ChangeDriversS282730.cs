using CarChanger.Common.Components;
using CarChanger.Common.Configs;
using CarChanger.Game.Components;
using DV.Wheels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game.AdditionalChanges
{
    internal class ChangeDriversS282730 : IAdditionalChange
    {
        private IEnumerable<Renderer>? _ogDrivers1;
        private IEnumerable<Renderer>? _ogDrivers2;
        private IEnumerable<Renderer>? _ogDrivers3;
        private IEnumerable<Renderer>? _ogDrivers4;
        private IEnumerable<Renderer>? _ogAxlesF;
        private IEnumerable<Renderer>? _ogAxlesR;
        private List<GameObject> _custom;
        private ChangeObject? _swivelF;
        private ChangeObject? _swivelR;
        private ExplosionModelHandler? _explosionHandler;

        private IEnumerable<Renderer> AllOriginalAxles
        {
            get
            {
                if (_ogDrivers1 != null)
                {
                    foreach (var item in _ogDrivers1)
                    {
                        yield return item;
                    }
                }
                if (_ogDrivers2 != null)
                {
                    foreach (var item in _ogDrivers2)
                    {
                        yield return item;
                    }
                }
                if (_ogDrivers3 != null)
                {
                    foreach (var item in _ogDrivers3)
                    {
                        yield return item;
                    }
                }
                if (_ogDrivers4 != null)
                {
                    foreach (var item in _ogDrivers4)
                    {
                        yield return item;
                    }
                }
                if (_ogAxlesF != null)
                {
                    foreach (var item in _ogAxlesF)
                    {
                        yield return item;
                    }
                }
                if (_ogAxlesR != null)
                {
                    foreach (var item in _ogAxlesR)
                    {
                        yield return item;
                    }
                }
            }
        }

        public ChangeDriversS282730(TrainCar car, LocoS282730AConfig config, MaterialHolder holder)
        {
            _custom = new List<GameObject>();

            // Drivers.
            if (config.UseCustomDrivers)
            {
                _ogDrivers1 = car.transform.AllNonNullFind(
                    "LocoS282A_Body/MovingParts_LOD0/LocoS282A_Drivetrain L/s282_wheels_driving_1",
                    "LocoS282A_Body/MovingParts_LOD0/LocoS282A_Drivetrain R/s282_wheels_driving_1")
                    .Select(x => x.GetComponent<Renderer>());
                _ogDrivers2 = car.transform.AllNonNullFind(
                    "LocoS282A_Body/MovingParts_LOD0/LocoS282A_Drivetrain L/s282_wheels_driving_2",
                    "LocoS282A_Body/MovingParts_LOD0/LocoS282A_Drivetrain R/s282_wheels_driving_2")
                    .Select(x => x.GetComponent<Renderer>());
                _ogDrivers3 = car.transform.AllNonNullFind(
                    "LocoS282A_Body/MovingParts_LOD0/LocoS282A_Drivetrain L/s282_wheels_driving_3",
                    "LocoS282A_Body/MovingParts_LOD0/LocoS282A_Drivetrain R/s282_wheels_driving_3")
                    .Select(x => x.GetComponent<Renderer>());
                _ogDrivers4 = car.transform.AllNonNullFind(
                    "LocoS282A_Body/MovingParts_LOD0/LocoS282A_Drivetrain L/s282_wheels_driving_4",
                    "LocoS282A_Body/MovingParts_LOD0/LocoS282A_Drivetrain R/s282_wheels_driving_4")
                    .Select(x => x.GetComponent<Renderer>());

                Instantiate(_ogDrivers1, config.Driver1);
                Instantiate(_ogDrivers2, config.Driver2);
                Instantiate(_ogDrivers3, config.Driver3);
                Instantiate(_ogDrivers4, config.Driver4);
            }

            // Front and rear axles.
            var rotations = car.GetComponentsInChildren<WheelRotationViaCode>(true);

            if (config.UseCustomFrontAxle)
            {
                var rotation = rotations.First(x => x.wheelRadius < 0.45f);
                _ogAxlesF = rotation.transformsToRotate.SelectMany(x => x.GetComponentsInChildren<Renderer>(true));

                InstantiateChild(rotation.transformsToRotate, config.FrontAxle);
            }

            if (config.UseCustomRearAxle)
            {
                var rotation = rotations.First(x => x.wheelRadius >= 0.45f);
                _ogAxlesR = rotation.transformsToRotate.SelectMany(x => x.GetComponentsInChildren<Renderer>(true));

                InstantiateChild(rotation.transformsToRotate, config.RearAxle);
            }

            _swivelF = new ChangeObject(car.transform.Find("Axle_F/bogie_car"), config.FrontSwivel, new GameObject[0], false, holder);
            _swivelR = new ChangeObject(car.transform.Find("Axle_R/bogie_car"), config.RearSwivel, new GameObject[0], false, holder);

            // Explosion.
            var disableGos = new List<DisableGameObjectOnExplosion>();
            var goSwaps = new List<SwapGameObjectOnExplosion>();
            var matSwaps = new List<SwapMaterialOnExplosion>();

            foreach (var item in _custom)
            {
                CarChangerExplosionManager.GetEntries(item, out var disableGosTemp, out var goSwapsTemp, out var matSwapsTemp);
                disableGos.AddRange(disableGosTemp);
                goSwaps.AddRange(goSwapsTemp);
                matSwaps.AddRange(matSwapsTemp);
            }

            _explosionHandler = CarChangerExplosionManager.PrepareExplosionHandlerFromEntries(holder, disableGos, goSwaps, matSwaps);

            // Locals.
            void Instantiate(IEnumerable<Renderer> renderers, GameObject? toInstantiate)
            {
                foreach (var item in renderers)
                {
                    item.enabled = false;
                    item.transform.SetChildrenActive(false);

                    if (toInstantiate != null)
                    {
                        var instance = Object.Instantiate(toInstantiate, item.transform);
                        instance.transform.ResetLocal();
                        ComponentProcessor.ProcessComponentsMinimal(instance, holder);
                        _custom.Add(instance);
                    }
                }
            }

            void InstantiateChild(IEnumerable<Transform> transforms, GameObject? toInstantiate)
            {
                foreach (var item in transforms)
                {
                    item.SetChildrenActive(false);

                    if (toInstantiate != null)
                    {
                        var instance = Object.Instantiate(toInstantiate, item);
                        instance.transform.ResetLocal();
                        ComponentProcessor.ProcessComponentsMinimal(instance, holder);
                        _custom.Add(instance);
                    }
                }
            }
        }

        public void Reset()
        {
            foreach (var item in AllOriginalAxles)
            {
                item.enabled = true;
                item.transform.SetChildrenActive(true);
            }

            foreach (var item in _custom)
            {
                Helpers.DestroyIfNotNull(item);
            }

            _swivelF?.Clear();
            _swivelR?.Clear();

            Helpers.DestroyGameObjectIfNotNull(_explosionHandler);
        }
    }
}
