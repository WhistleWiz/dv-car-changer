using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.InteractablesChanges
{
    internal class LocoDH4670InteractablesChanger : IInteractablesChanger
    {
        private LocoDH4670Config _config;
        private MaterialHolder _materialHolder;
        private ChangeObject? _doorL;
        private ChangeObject? _doorR;
        private ChangeObject? _windowL;
        private ChangeObject? _windowR;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public LocoDH4670InteractablesChanger(LocoDH4670Config config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject? interactables)
        {
            if (interactables == null) return;

            var doorL = interactables.transform.Find("DoorsWindows/DoorL/C_DoorL");
            var doorR = interactables.transform.Find("DoorsWindows/DoorR/C_DoorR");
            var windowL = interactables.transform.Find("DoorsWindows/WindowSlideL/WindowSlideL pivot/C_WindowSlideL");
            var windowR = interactables.transform.Find("DoorsWindows/WindowSlideR/WindowSlideR pivot/C_WindowSlideR");

            _doorL = new ChangeObject(doorL, IsExploded ? _config.DoorLeftExploded : _config.DoorLeft, new[]
                {
                    doorL.Find("dh4_exterior_door_L").gameObject,
                    doorL.Find("dh4_cab_door_L").gameObject,
                    doorL.Find("dh4_cab_door_L_window").gameObject,
                    doorL.Find("dh4_exterior_door_L_LOD").gameObject
                },
                _config.HideOriginalDoors, _materialHolder);
            _doorR = new ChangeObject(doorR, IsExploded ? _config.DoorRightExploded : _config.DoorRight, new[]
                {
                    doorR.Find("dh4_exterior_door_R").gameObject,
                    doorR.Find("dh4_cab_door_R").gameObject,
                    doorR.Find("dh4_cab_door_R_window").gameObject,
                    doorR.Find("dh4_exterior_door_R_LOD").gameObject
                },
                _config.HideOriginalDoors, _materialHolder);
            _windowL = new ChangeObject(windowL, IsExploded ? _config.WindowLeftExploded : _config.WindowLeft, new[]
                {
                    windowL.Find("dh4_window_farme_L").gameObject,
                    windowL.Find("dh4_window_glass_L").gameObject
                },
                _config.HideOriginalWindows, _materialHolder);
            _windowR = new ChangeObject(windowR, IsExploded ? _config.WindowRightExploded : _config.WindowRight, new[]
                {
                    windowR.Find("dh4_window_frame_R").gameObject,
                    windowR.Find("dh4_window_frame_R").gameObject
                },
                _config.HideOriginalWindows, _materialHolder);

            _config.InteractablesApplied(interactables, IsExploded);
        }

        public void Unapply(GameObject? interactables)
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
