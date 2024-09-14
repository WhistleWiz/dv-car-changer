using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.InteractablesChanges
{
    internal class CabooseInteractablesChanger : IInteractablesChanger
    {
        private CabooseConfig _config;
        private MaterialHolder _materialHolder;
        private ChangeObject _doorF = null!;
        private ChangeObject _doorR = null!;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public CabooseInteractablesChanger(CabooseConfig config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject? interactables)
        {
            if (interactables == null) return;

            var doorF = interactables.transform.Find("DoorsAndWindows/C CabooseDoor1");
            var doorR = interactables.transform.Find("DoorsAndWindows/C CabooseDoor2");

            _doorF = new ChangeObject(doorF, _config.DoorFront, new[]
                {
                    doorF.Find("CabooseInteriorDoor1").gameObject,
                    doorF.Find("CabooseInteriorDoor1").gameObject,
                    doorF.Find("CabooseGlassDoor1").gameObject,
                    doorF.Find("CabooseExteriorDoor1_LOD").gameObject
                },
                _config.HideOriginalDoors, _materialHolder);
            _doorR = new ChangeObject(doorR, _config.DoorRear, new[]
                {
                    doorR.Find("CabooseInteriorDoor2").gameObject,
                    doorR.Find("CabooseInteriorDoor2").gameObject,
                    doorR.Find("CabooseGlassDoor2").gameObject,
                    doorR.Find("CabooseExteriorDoor2_LOD").gameObject
                },
                _config.HideOriginalDoors, _materialHolder);

            _config.InteractablesApplied(interactables, IsExploded);
        }

        public void Unapply(GameObject? interactables)
        {
            if (interactables == null) return;

            _doorF.Clear();
            _doorR.Clear();

            _config.InteractablesUnapplied(interactables);
        }
    }
}
