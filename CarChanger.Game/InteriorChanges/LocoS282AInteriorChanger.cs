using CarChanger.Common.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CarChanger.Game.InteriorChanges
{
    internal class LocoS282AInteriorChanger : IInteriorChanger
    {
        private LocoS282AConfig _config;
        private MaterialHolder _materialHolder;

        public LocoS282AInteriorChanger(LocoS282AConfig config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject interior)
        {
            throw new NotImplementedException();
        }

        public void Unapply(GameObject interior)
        {
            throw new NotImplementedException();
        }
    }
}
