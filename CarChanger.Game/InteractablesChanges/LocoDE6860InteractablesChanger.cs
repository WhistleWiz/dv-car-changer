using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.InteractablesChanges
{
    internal class LocoDE6860InteractablesChanger : IInteractablesChanger
    {
        private LocoDE6860Config _config;
        private MaterialHolder _materialHolder;
        private ChangeObject? _engR;
        private ChangeObject? _engL;
        private ChangeObject? _cabR;
        private ChangeObject? _cabF;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public LocoDE6860InteractablesChanger(LocoDE6860Config config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject? interactables)
        {
            if (interactables == null) return;

            var engR = interactables.transform.Find("DoorsWindows/DoorEngineR/C_DoorEngineR");
            var engL = interactables.transform.Find("DoorsWindows/DoorEngineL/C_DoorEngineL");
            var cabR = interactables.transform.Find("DoorsWindows/DoorR/C_DoorR");
            var cabF = interactables.transform.Find("DoorsWindows/DoorF/C_DoorF");

            _engR = new ChangeObject(engR, IsExploded ? _config.EngineDoorRightExploded : _config.EngineDoorRight, new[]
                {
                    engR.Find("engine_bay_door01").gameObject,
                    engR.Find("engine_bay_door01_LOD").gameObject
                },
                _config.HideOriginalEngineDoors, _materialHolder);
            _engL = new ChangeObject(engL, IsExploded ? _config.EngineDoorLeftExploded : _config.EngineDoorLeft, new[]
                {
                    engL.Find("engine_bay_door02").gameObject,
                    engL.Find("engine_bay_door02_LOD").gameObject
                },
                _config.HideOriginalEngineDoors, _materialHolder);
            _cabR = new ChangeObject(cabR, IsExploded ? _config.CabDoorRearExploded : _config.CabDoorRear, new[]
                {
                    cabR.Find("cab_door01a").gameObject,
                    cabR.Find("cab_door01a_LOD").gameObject,
                    cabR.Find("cab_door01b").gameObject,
                    cabR.Find("cab_door01b_window").gameObject
                },
                _config.HideOriginalCabDoors, _materialHolder);
            _cabF = new ChangeObject(cabF, IsExploded ? _config.CabDoorFrontExploded : _config.CabDoorFront, new[]
                {
                    cabF.Find("cab_door02a").gameObject,
                    cabF.Find("cab_door02a_LOD").gameObject,
                    cabF.Find("cab_door02b").gameObject,
                    cabF.Find("cab_door02b_window").gameObject
                },
                _config.HideOriginalCabDoors, _materialHolder);

            _config.InteractablesApplied(interactables, IsExploded);
        }

        public void Unapply(GameObject? interactables)
        {
            if (interactables == null) return;

            _engR?.Clear();
            _engL?.Clear();
            _cabR?.Clear();
            _cabF?.Clear();

            _config.InteractablesUnapplied(interactables);
        }
    }
}
