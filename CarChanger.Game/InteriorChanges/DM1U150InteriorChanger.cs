using CarChanger.Common.Configs;
using System.IO;
using UnityEngine;

namespace CarChanger.Game.InteriorChanges
{
    internal class DM1U150InteriorChanger : BasicInteriorChanger
    {
        private ChangeObject? _door;
        private LocoDM1U150Config _config;

        public DM1U150InteriorChanger(LocoDM1U150Config config, MaterialHolder matHolder) :
            base(config, matHolder,
                ("cab", true),
                ("dash", config.HideDash))
        {
            _config = config;
        }

        public override void Apply(GameObject? interior)
        {
            // Interior unloaded, so don't apply.
            if (interior == null) return;

            var door = interior.transform.Find("C_InteriorDoor");

            _door = new ChangeObject(door, _config.DoorInterior, new[]
                {
                    door.Find("dm1u-150_interior_door").gameObject,
                },
                _config.HideOriginalInteriorDoor, MaterialHolder);

            base.Apply(interior);
        }

        public override void Unapply(GameObject? interior)
        {
            // Already gone, skip.
            if (interior == null) return;

            _door?.Clear();

            base.Unapply(interior);
        }
    }
}
