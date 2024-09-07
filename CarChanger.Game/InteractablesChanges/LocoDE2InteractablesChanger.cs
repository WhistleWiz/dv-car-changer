using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.InteractablesChanges
{
    internal class LocoDE2InteractablesChanger : IInteractablesChanger
    {
        private LocoDE2Config _config;
        private MaterialHolder _materialHolder;
        private ChangeObject? _doorR;
        private ChangeObject? _doorF;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public LocoDE2InteractablesChanger(LocoDE2Config config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject interactables)
        {
            if (interactables == null) return;

            var doorR = interactables.transform.Find("DoorsWindows/C_DoorR");
            var doorF = interactables.transform.Find("DoorsWindows/C_DoorF");

            _doorR = new ChangeObject(doorR, IsExploded ? _config.DoorRearExploded : _config.DoorRear, new[]
                {
                    doorR.Find("ext cab_door1a").gameObject,
                    doorR.Find("ext cab_door1a_LOD").gameObject,
                    doorR.Find("cab_door1b").gameObject,
                    doorR.Find("cab_door1b_window").gameObject
                },
                _config.HideOriginalCabDoors, _materialHolder);
            _doorF = new ChangeObject(doorF, IsExploded ? _config.DoorFrontExploded : _config.DoorFront, new[]
                {
                    doorF.Find("ext cab_door2a").gameObject,
                    doorF.Find("ext cab_door2a_LOD").gameObject,
                    doorF.Find("cab_door2b").gameObject,
                    doorF.Find("cab_door2b_window").gameObject
                },
                _config.HideOriginalCabDoors, _materialHolder);

            _config.InteractablesApplied(interactables, IsExploded);
        }

        public void Unapply(GameObject interactables)
        {
            if (interactables == null) return;

            _doorR?.Clear();
            _doorF?.Clear();

            _config.InteractablesUnapplied(interactables);
        }
    }
}
