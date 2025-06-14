using CarChanger.Common.Components;
using CarChanger.Common.Configs;
using CarChanger.Game.Components;
using System.Collections.Generic;
using System.Linq;

namespace CarChanger.Game.AdditionalChanges
{
    internal class ChangeDriversDM3540 : IAdditionalChange
    {
        private ChangeObject? _ogDriverF;
        private ChangeObject? _ogDriverC;
        private ChangeObject? _ogDriverR;
        private ExplosionModelHandler? _explosionHandler;

        public ChangeDriversDM3540(TrainCar car, LocoDM3540Config config, MaterialHolder holder)
        {
            if (!config.UseCustomDrivers) return;

            var tf = car.transform.Find("DriveGear/axleF");
            var tc = car.transform.Find("DriveGear/axleC");
            var tr = car.transform.Find("DriveGear/axleR");

            _ogDriverF = new ChangeObject(tf, config.DriverF, tf.AllChildGOs().ToArray(), true, holder);
            _ogDriverC = new ChangeObject(tc, config.DriverC, tc.AllChildGOs().ToArray(), true, holder);
            _ogDriverR = new ChangeObject(tr, config.DriverR, tr.AllChildGOs().ToArray(), true, holder);

            var disableGos = new List<DisableGameObjectOnExplosion>();
            var goSwaps = new List<SwapGameObjectOnExplosion>();
            var matSwaps = new List<SwapMaterialOnExplosion>();

            if (_ogDriverF.Instanced != null)
            {
                CarChangerExplosionManager.GetEntries(_ogDriverF.Instanced, out var disableGosTemp, out var goSwapsTemp, out var matSwapsTemp);
                disableGos.AddRange(disableGosTemp);
                goSwaps.AddRange(goSwapsTemp);
                matSwaps.AddRange(matSwapsTemp);
            }

            if (_ogDriverC.Instanced != null)
            {
                CarChangerExplosionManager.GetEntries(_ogDriverC.Instanced, out var disableGosTemp, out var goSwapsTemp, out var matSwapsTemp);
                disableGos.AddRange(disableGosTemp);
                goSwaps.AddRange(goSwapsTemp);
                matSwaps.AddRange(matSwapsTemp);
            }

            if (_ogDriverR.Instanced != null)
            {
                CarChangerExplosionManager.GetEntries(_ogDriverR.Instanced, out var disableGosTemp, out var goSwapsTemp, out var matSwapsTemp);
                disableGos.AddRange(disableGosTemp);
                goSwaps.AddRange(goSwapsTemp);
                matSwaps.AddRange(matSwapsTemp);
            }

            _explosionHandler = CarChangerExplosionManager.PrepareExplosionHandlerFromEntries(holder, disableGos, goSwaps, matSwaps);
        }

        public void Reset()
        {
            _ogDriverF?.Clear();
            _ogDriverC?.Clear();
            _ogDriverR?.Clear();

            Helpers.DestroyGameObjectIfNotNull(_explosionHandler);
        }
    }
}
