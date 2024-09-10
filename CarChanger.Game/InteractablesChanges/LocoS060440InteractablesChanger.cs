using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.InteractablesChanges
{
    internal class LocoS060440InteractablesChanger : IInteractablesChanger
    {
        private LocoS060440Config _config;
        private MaterialHolder _materialHolder;
        private ChangeObject? _doorL;
        private ChangeObject? _doorR;
        private ChangeObject? _windowL;
        private ChangeObject? _windowR;
        private ChangeObject? _sunroof;
        private ChangeObject? _hatchL;
        private ChangeObject? _hatchR;
        private ChangeObject? _waterL;
        private ChangeObject? _waterR;
        private IndicatorModelChangerHelper? _coal;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public LocoS060440InteractablesChanger(LocoS060440Config config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject interactables)
        {
            if (interactables == null) return;

            var doorL = interactables.transform.Find("Interactables/Doors/DoorL/C_DoorL");
            var doorR = interactables.transform.Find("Interactables/Doors/DoorR/C_DoorR");
            var windowL = interactables.transform.Find("Interactables/Windows/WindowSlideL/WindowSlideL_pivot/C_WindowSlideL");
            var windowR = interactables.transform.Find("Interactables/Windows/WindowSlideR/WindowSlideR_pivot/C_WindowSlideR");
            var sunroof = interactables.transform.Find("Interactables/Sunroof/Sunroof_pivot/C_Sunroof");
            var hatchL = interactables.transform.Find("Interactables/WaterHatchL/C_WaterHatchL");
            var hatchR = interactables.transform.Find("Interactables/WaterHatchR/C_WaterHatchR");
            var waterL = interactables.transform.Find("Indicators/I_WaterStorageL/scaler");
            var waterR = interactables.transform.Find("Indicators/I_WaterStorageR/scaler");

            _doorL = new ChangeObject(doorL, IsExploded ? _config.DoorLeftExploded : _config.DoorLeft, new[]
                {
                    doorL.Find("model").gameObject,
                    doorL.Find("s060_ext_door_L").gameObject
                },
                _config.HideOriginalDoors, _materialHolder);
            _doorR = new ChangeObject(doorR, IsExploded ? _config.DoorRightExploded : _config.DoorRight, new[]
                {
                    doorR.Find("model").gameObject,
                    doorR.Find("s060_ext_door_R").gameObject
                },
                _config.HideOriginalDoors, _materialHolder);
            _windowL = new ChangeObject(windowL, IsExploded ? _config.WindowLeftExploded : _config.WindowLeft, new[]
                {
                    windowL.Find("s060_cab_window_frame_L").gameObject,
                    windowL.Find("s060_cab_window_glass_L").gameObject
                },
                _config.HideOriginalWindows, _materialHolder);
            _windowR = new ChangeObject(windowR, IsExploded ? _config.WindowRightExploded : _config.WindowRight, new[]
                {
                    windowR.Find("s060_cab_window_frame_R").gameObject,
                    windowR.Find("s060_cab_window_glass_R").gameObject
                },
                _config.HideOriginalWindows, _materialHolder);
            _sunroof = new ChangeObject(sunroof, IsExploded ? _config.SunroofExploded : _config.Sunroof, new[]
                {
                    sunroof.Find("s060_ext_sunroof").gameObject,
                    sunroof.Find("s060_cab_sunroof").gameObject
                },
                _config.HideOriginalSunroof, _materialHolder);
            _hatchL = new ChangeObject(hatchL, IsExploded ? _config.WaterHatchLeftExploded : _config.WaterHatchLeft, new[]
                {
                    hatchL.Find("model").gameObject
                },
                _config.HideOriginalHatches, _materialHolder);
            _hatchR = new ChangeObject(hatchR, IsExploded ? _config.WaterHatchRightExploded : _config.WaterHatchRight, new[]
                {
                    hatchR.Find("model").gameObject
                },
                _config.HideOriginalHatches, _materialHolder);
            _waterL = new ChangeObject(waterL, _config.WaterLeft, new[]
                {
                    waterL.Find("water").gameObject
                },
                _config.HideOriginalWater, _materialHolder);
            _waterR = new ChangeObject(waterR, _config.WaterRight, new[]
                {
                    waterR.Find("water").gameObject
                },
                _config.HideOriginalWater, _materialHolder);

            if (_config.ReplaceCoal)
            {
                _coal = new IndicatorModelChangerHelper(
                    interactables.transform.Find("Indicators/I_CoalStorage").GetComponent<IndicatorModelChanger>(),
                    _config.CoalModels,
                    _config.SwitchPercentage);
            }

            _config.InteractablesApplied(interactables, IsExploded);
        }

        public void Unapply(GameObject interactables)
        {
            if (interactables == null) return;

            _doorL?.Clear();
            _doorR?.Clear();
            _windowL?.Clear();
            _windowR?.Clear();
            _sunroof?.Clear();
            _hatchL?.Clear();
            _hatchR?.Clear();
            _waterL?.Clear();
            _waterR?.Clear();
            _coal?.Clear();

            _config.InteractablesUnapplied(interactables);
        }
    }
}
