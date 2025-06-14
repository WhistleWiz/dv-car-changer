using CarChanger.Common.Configs;
using System.Linq;

namespace CarChanger.Game.AdditionalChanges
{
    internal class ChangeDriversDM3540 : IAdditionalChange
    {
        private ChangeObject? _ogDriverF;
        private ChangeObject? _ogDriverC;
        private ChangeObject? _ogDriverR;

        public ChangeDriversDM3540(TrainCar car, LocoDM3540Config config, MaterialHolder mats)
        {
            if (config.UseCustomDrivers)
            {
                var tf = car.transform.Find("DriveGear/axleF");
                var tc = car.transform.Find("DriveGear/axleC");
                var tr = car.transform.Find("DriveGear/axleR");

                _ogDriverF = new ChangeObject(tf, config.DriverF, tf.AllChildGOs().ToArray(), true, mats);
                _ogDriverC = new ChangeObject(tf, config.DriverC, tc.AllChildGOs().ToArray(), true, mats);
                _ogDriverR = new ChangeObject(tf, config.DriverR, tr.AllChildGOs().ToArray(), true, mats);
            }
        }

        public void Reset()
        {
            _ogDriverF?.Clear();
            _ogDriverC?.Clear();
            _ogDriverR?.Clear();
        }
    }
}
