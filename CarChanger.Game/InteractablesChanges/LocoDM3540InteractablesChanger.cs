using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.InteractablesChanges
{
    internal class LocoDM3540InteractablesChanger : IInteractablesChanger
    {
        private LocoDM3540Config _config;
        private MaterialHolder _materialHolder;
        private ChangeObject? _doorL;
        private ChangeObject? _doorR;
        private ChangeObject? _windowL;
        private ChangeObject? _windowR;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public LocoDM3540InteractablesChanger(LocoDM3540Config config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject interactables)
        {
            if (interactables == null) return;

            var doorL = interactables.transform.Find("DoorsWindows/C_DoorL");
            var doorR = interactables.transform.Find("DoorsWindows/C_DoorR");
            var windowL = interactables.transform.Find("DoorsWindows/WindowSlideL/WindowSlideL pivot/C_WindowSlideL");
            var windowR = interactables.transform.Find("DoorsWindows/WindowSlideR/WindowSlideR pivot/C_WindowSlideR");

            _doorL = new ChangeObject(doorL, IsExploded ? _config.DoorLeftExploded : _config.DoorLeft, new[]
                {
                    doorL.Find("ext cab_door2a").gameObject,
                    doorL.Find("cab_door2b").gameObject,
                    doorL.Find("cab_door2b_window").gameObject,
                    doorL.Find("ext cab_door2a_LOD").gameObject,
                    doorL.Find("cab_door2a_LOD").gameObject
                },
                _config.HideOriginalDoors, _materialHolder);
            _doorR = new ChangeObject(doorR, IsExploded ? _config.DoorRightExploded : _config.DoorRight, new[]
                {
                    doorL.Find("ext cab_door1a").gameObject,
                    doorL.Find("cab_door1b").gameObject,
                    doorL.Find("cab_door1b_window").gameObject,
                    doorL.Find("ext cab_door1a_LOD").gameObject,
                    doorL.Find("cab_door1a_LOD").gameObject
                },
                _config.HideOriginalDoors, _materialHolder);
            _windowL = new ChangeObject(windowL, IsExploded ? _config.WindowLeftExploded : _config.WindowLeft, new[]
                {
                    windowL.Find("cab_window_frame_L").gameObject
                },
                _config.HideOriginalWindows, _materialHolder);
            _windowR = new ChangeObject(windowR, IsExploded ? _config.WindowRightExploded : _config.WindowRight, new[]
                {
                    windowR.Find("cab_window_frame_R").gameObject
                },
                _config.HideOriginalWindows, _materialHolder);

            _config.InteractablesApplied(interactables, IsExploded);
        }

        public void Unapply(GameObject interactables)
        {
            if (interactables == null) return;

            _doorL?.Clear();
            _doorR?.Clear();
            _windowL?.Clear();
            _windowR?.Clear();

            _config.InteractablesUnapplied(interactables);
        }
    }
}
