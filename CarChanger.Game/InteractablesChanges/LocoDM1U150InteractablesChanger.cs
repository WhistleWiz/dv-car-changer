using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.InteractablesChanges
{
    internal class LocoDM1U150InteractablesChanger : IInteractablesChanger
    {
        private LocoDM1U150Config _config;
        private MaterialHolder _materialHolder;
        private ChangeObject? _doorR;

        public LocoDM1U150InteractablesChanger(LocoDM1U150Config config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject? interactables)
        {
            if (interactables == null) return;

            var doorR = interactables.transform.Find("DoorsWindows/C_DoorRear");

            _doorR = new ChangeObject(doorR, _config.DoorRear, new[]
                {
                    doorR.Find("dm1u-150_door").gameObject,
                    doorR.Find("dm1u-150_door_LOD1").gameObject,
                    doorR.Find("dm1u-150_window_door").gameObject
                },
                _config.HideOriginalDoors, _materialHolder);

            _config.InteractablesApplied(interactables, false);
        }

        public void Unapply(GameObject? interactables)
        {
            if (interactables == null) return;

            _doorR?.Clear();

            _config.InteractablesUnapplied(interactables);
        }
    }
}
