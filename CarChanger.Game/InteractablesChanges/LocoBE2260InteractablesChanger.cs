using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.InteractablesChanges
{
    internal class LocoBE2260InteractablesChanger : IInteractablesChanger
    {
        private LocoBE2260Config _config;
        private MaterialHolder _materialHolder;
        private ChangeObject? _door;
        private ChangeObject? _windowF;
        private ChangeObject? _windowR;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public LocoBE2260InteractablesChanger(LocoBE2260Config config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject interactables)
        {
            if (interactables == null) return;

            var door = interactables.transform.Find("DoorsWindows/Door/C_Door");
            var windowF = interactables.transform.Find("DoorsWindows/WindowF/Window pivot/C_WindowF");
            var windowR = interactables.transform.Find("DoorsWindows/WindowR/Window pivot/C_WindowR");

            _door = new ChangeObject(door, IsExploded ? _config.DoorExploded : _config.Door, new[]
                {
                    door.Find("microshunter_ext_door").gameObject,
                    door.Find("microshunter_int_door").gameObject,
                    door.Find("microshunter_door_window").gameObject,
                    door.Find("microshunter_ext_door_LOD").gameObject
                },
                _config.HideOriginalCabDoor, _materialHolder);
            _windowF = new ChangeObject(windowF, IsExploded ? _config.WindowFrontExploded : _config.WindowFront, new[]
                {
                    windowF.Find("microshunter_int_window_F").gameObject,
                    windowF.Find("microshunter_ext_window_F").gameObject,
                    windowF.Find("microshunter_window_FC").gameObject
                },
                _config.HideOriginalWindows, _materialHolder);
            _windowR = new ChangeObject(windowR, IsExploded ? _config.WindowRearExploded : _config.WindowRear, new[]
                {
                    windowR.Find("microshunter_int_window_R").gameObject,
                    windowR.Find("microshunter_ext_window_R").gameObject,
                    windowR.Find("microshunter_window_RC").gameObject
                },
                _config.HideOriginalWindows, _materialHolder);

            _config.InteractablesApplied(interactables, IsExploded);
        }

        public void Unapply(GameObject interactables)
        {
            if (interactables == null) return;

            _door?.Clear();
            _windowF?.Clear();
            _windowR?.Clear();

            _config.InteractablesUnapplied(interactables);
        }
    }
}
