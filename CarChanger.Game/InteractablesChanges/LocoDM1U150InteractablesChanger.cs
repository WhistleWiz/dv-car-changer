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

            var doorR = interactables.transform.Find("DoorsWindows/DoorR/C_DoorR");

            _doorR = new ChangeObject(doorR, _config.DoorRear, new[]
                {
                    doorR.Find("dh4_exterior_door_R").gameObject,
                    doorR.Find("dh4_cab_door_R").gameObject,
                    doorR.Find("dh4_cab_door_R_window").gameObject,
                    doorR.Find("dh4_exterior_door_R_LOD").gameObject
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
