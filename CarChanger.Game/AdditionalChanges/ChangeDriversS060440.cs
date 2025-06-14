using CarChanger.Common.Components;
using CarChanger.Common.Configs;
using CarChanger.Game.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game.AdditionalChanges
{
    internal class ChangeDriversS060440 : IAdditionalChange
    {
        private IEnumerable<Renderer>? _ogDrivers1;
        private IEnumerable<Renderer>? _ogDrivers2;
        private IEnumerable<Renderer>? _ogDrivers3;
        private List<GameObject> _custom;
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
            }
        }

        public ChangeDriversS060440(TrainCar car, LocoS060440Config config, MaterialHolder holder)
        {
            _custom = new List<GameObject>();

            if (config.UseCustomDrivers)
            {
                _ogDrivers1 = car.transform.AllNonNullFind(
                    "LocoS060_Body/DrivingMechanism_L/s060_Wheels_01",
                    "LocoS060_Body/DrivingMechanism_R/s060_Wheels_01")
                    .Select(x => x.GetComponent<Renderer>());
                _ogDrivers2 = car.transform.AllNonNullFind(
                    "LocoS060_Body/DrivingMechanism_L/s060_Wheels_02",
                    "LocoS060_Body/DrivingMechanism_R/s060_Wheels_02")
                    .Select(x => x.GetComponent<Renderer>());
                _ogDrivers3 = car.transform.AllNonNullFind(
                    "LocoS060_Body/DrivingMechanism_L/s060_Wheels_03",
                    "LocoS060_Body/DrivingMechanism_R/s060_Wheels_03")
                    .Select(x => x.GetComponent<Renderer>());

                Instantiate(_ogDrivers1, config.Driver1);
                Instantiate(_ogDrivers2, config.Driver2);
                Instantiate(_ogDrivers3, config.Driver3);
            }

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

            Helpers.DestroyGameObjectIfNotNull(_explosionHandler);
        }
    }
}
