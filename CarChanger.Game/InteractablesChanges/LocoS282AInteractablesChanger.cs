using CarChanger.Common.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CarChanger.Game.InteractablesChanges
{
    internal class LocoS282AInteractablesChanger : IInteractablesChanger
    {
        private LocoS282AConfig _config;
        private MaterialHolder _materialHolder;

        public LocoS282AInteractablesChanger(LocoS282AConfig config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject interactables)
        {
            throw new NotImplementedException();
        }

        public void Unapply(GameObject interactables)
        {
            throw new NotImplementedException();
        }
    }
}
