﻿using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.InteractablesChanges
{
    internal class LocoS282730AInteractablesChanger : IInteractablesChanger
    {
        private LocoS282730AConfig _config;
        private MaterialHolder _materialHolder;
        private ChangeObject? _windowR;
        private ChangeObject? _windowL;
        private ChangeObject? _toolbox;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public LocoS282730AInteractablesChanger(LocoS282730AConfig config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject? interactables)
        {
            if (interactables == null) return;

            var wR = interactables.transform.Find("Interactables/WindowR/C_WindowR");
            var wL = interactables.transform.Find("Interactables/WindowL/C_WindowL");
            var toolbox = interactables.transform.Find("Interactables/Toolbox/C_Toolbox");

            _windowR = new ChangeObject(wR, IsExploded ? _config.RightWindowExploded : _config.RightWindow, new[]
                {
                    wR.transform.Find("model").gameObject
                },
                _config.HideOriginalWindows, _materialHolder);
            _windowL = new ChangeObject(wL, IsExploded ? _config.LeftWindowExploded : _config.LeftWindow, new[]
                {
                    wL.transform.Find("model").gameObject
                },
                _config.HideOriginalWindows, _materialHolder);
            _toolbox = new ChangeObject(toolbox, IsExploded ? _config.ToolboxLidExploded : _config.ToolboxLid, new[]
                {
                    toolbox.transform.Find("model").gameObject
                },
                _config.HideOriginalToolboxLid, _materialHolder);

            _config.InteractablesApplied(interactables, IsExploded);
        }

        public void Unapply(GameObject? interactables)
        {
            if (interactables == null) return;

            _windowR?.Clear();
            _windowL?.Clear();
            _toolbox?.Clear();

            _config.InteractablesUnapplied(interactables);
        }
    }
}
