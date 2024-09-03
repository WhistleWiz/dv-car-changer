using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.InteractablesChanges
{
    internal class LocoDE6InteractablesChanger : IInteractablesChanger
    {
        private LocoDE6Config _config;
        private MaterialHolder _materialHolder;
        private GameObject _engR = null!;
        private GameObject _engL = null!;
        private GameObject _cabR = null!;
        private GameObject _cabF = null!;
        private GameObject[] _engOg = null!;
        private GameObject[] _cabOg = null!;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public LocoDE6InteractablesChanger(LocoDE6Config config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject interactables)
        {
            if (interactables == null) return;

            var engR = interactables.transform.Find("DoorsWindows/DoorEngineR/C_DoorEngineR");
            var engL = interactables.transform.Find("DoorsWindows/DoorEngineL/C_DoorEngineL");
            var cabR = interactables.transform.Find("DoorsWindows/DoorR/C_DoorR");
            var cabF = interactables.transform.Find("DoorsWindows/DoorF/C_DoorF");

            _engOg = new[]
            {
                engR.Find("engine_bay_door01").gameObject,
                engR.Find("engine_bay_door01_LOD").gameObject,
                engL.Find("engine_bay_door02").gameObject,
                engL.Find("engine_bay_door02_LOD").gameObject
            };

            _cabOg = new[]
            {
                cabR.Find("cab_door01a").gameObject,
                cabR.Find("cab_door01a_LOD").gameObject,
                cabR.Find("cab_door01b").gameObject,
                cabR.Find("cab_door01b_window").gameObject,
                cabF.Find("cab_door02a").gameObject,
                cabF.Find("cab_door02a_LOD").gameObject,
                cabF.Find("cab_door02b").gameObject,
                cabF.Find("cab_door02b_window").gameObject,
            };

            if (_config.HideOriginalEngineDoors)
            {
                foreach (var item in _engOg)
                {
                    item.SetActive(false);
                }
            }

            if (_config.HideOriginalCabDoors)
            {
                foreach (var item in _cabOg)
                {
                    item.SetActive(false);
                }
            }

            _engR = Helpers.InstantiateIfNotNull(IsExploded ? _config.EngineDoorRightExploded : _config.EngineDoorRight, engR);
            _engL = Helpers.InstantiateIfNotNull(IsExploded ? _config.EngineDoorLeftExploded : _config.EngineDoorLeft, engL);
            _cabR = Helpers.InstantiateIfNotNull(IsExploded ? _config.CabDoorRearExploded : _config.CabDoorRear, cabR);
            _cabF = Helpers.InstantiateIfNotNull(IsExploded ? _config.CabDoorFrontExploded : _config.CabDoorFront, cabF);

            ComponentProcessor.ProcessComponents(_engR, _materialHolder);
            ComponentProcessor.ProcessComponents(_engL, _materialHolder);
            ComponentProcessor.ProcessComponents(_cabR, _materialHolder);
            ComponentProcessor.ProcessComponents(_cabF, _materialHolder);

            _config.InteractablesApplied(interactables, IsExploded);
        }

        public void Unapply(GameObject interactables)
        {
            if (interactables == null) return;

            Helpers.DestroyIfNotNull(_engR);
            Helpers.DestroyIfNotNull(_engL);
            Helpers.DestroyIfNotNull(_cabR);
            Helpers.DestroyIfNotNull(_cabF);

            if (_config.HideOriginalEngineDoors)
            {
                foreach (var item in _engOg)
                {
                    if (item == null) continue;

                    item.SetActive(true);
                }
            }

            if (_config.HideOriginalCabDoors)
            {
                foreach (var item in _cabOg)
                {
                    if (item == null) continue;

                    item.SetActive(true);
                }
            }

            _config.InteractablesUnapplied(interactables);
        }
    }
}
