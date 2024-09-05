using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.InteractablesChanges
{
    internal class LocoS282AInteractablesChanger : IInteractablesChanger
    {
        private LocoS282AConfig _config;
        private MaterialHolder _materialHolder;
        private GameObject _windowR = null!;
        private GameObject _windowL = null!;
        private GameObject _toolbox = null!;
        private GameObject[] _ogWindows = null!;
        private GameObject _ogBox = null!;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public LocoS282AInteractablesChanger(LocoS282AConfig config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject interactables)
        {
            if (interactables == null) return;

            var wR = interactables.transform.Find("Interactables/WindowR/C_WindowR");
            var wL = interactables.transform.Find("Interactables/WindowL/C_WindowL");
            var toolbox = interactables.transform.Find("Interactables/Toolbox/C_Toolbox");

            _ogWindows = new[]
            {
                wR.transform.Find("model").gameObject,
                wL.transform.Find("model").gameObject
            };

            if (_config.HideOriginalWindows)
            {
                foreach (var item in _ogWindows)
                {
                    item.SetActive(false);
                }
            }

            _ogBox = toolbox.transform.Find("model").gameObject;

            if (_config.HideOriginalToolboxLid)
            {
                _ogBox.SetActive(false);
            }

            _windowR = Helpers.InstantiateIfNotNull(IsExploded ? _config.RightWindowExploded : _config.RightWindow, wR);
            _windowL = Helpers.InstantiateIfNotNull(IsExploded ? _config.LeftWindowExploded : _config.LeftWindow, wL);
            _toolbox = Helpers.InstantiateIfNotNull(IsExploded ? _config.ToolboxLidExploded : _config.ToolboxLid, toolbox);

            ComponentProcessor.ProcessComponents(_windowR, _materialHolder);
            ComponentProcessor.ProcessComponents(_windowL, _materialHolder);
            ComponentProcessor.ProcessComponents(_toolbox, _materialHolder);

            _config.InteractablesApplied(interactables, IsExploded);
        }

        public void Unapply(GameObject interactables)
        {
            if (interactables == null) return;

            Helpers.DestroyIfNotNull(_windowR);
            Helpers.DestroyIfNotNull(_windowL);
            Helpers.DestroyIfNotNull(_toolbox);

            if (_config.HideOriginalWindows)
            {
                foreach (var item in _ogWindows)
                {
                    item.SetActive(true);
                }
            }

            if (_config.HideOriginalToolboxLid)
            {
                _ogBox.SetActive(true);
            }

            _config.InteractablesUnapplied(interactables);
        }
    }
}
