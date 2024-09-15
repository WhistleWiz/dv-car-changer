using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.InteriorChanges
{
    internal class CustomCarInteriorChanger : IInteriorChanger
    {
        private CustomCarConfig _config;
        private MaterialHolder _materialHolder;
        private ChangeObject? _change;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public CustomCarInteriorChanger(CustomCarConfig config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject? interior)
        {
            if (interior == null) return;

            _change = new ChangeObject(interior.transform, IsExploded ? _config.InteriorPrefabExploded : _config.InteriorPrefab,
                new GameObject[0], false, _materialHolder);

            _config.InteriorApplied(interior, IsExploded);
        }

        public void Unapply(GameObject? interior)
        {
            if (interior == null) return;

            _change?.Clear();

            _config.InteriorUnapplied(interior);
        }
    }
}
