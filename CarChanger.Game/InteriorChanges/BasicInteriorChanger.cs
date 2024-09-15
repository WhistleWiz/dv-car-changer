using CarChanger.Common.Configs;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game.InteriorChanges
{
    internal class BasicInteriorChanger : IInteriorChanger
    {
        private CarWithInteriorConfig _config;
        private MaterialHolder _materialHolder;
        private ChangeObject? _cab;
        private string[] _paths;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public BasicInteriorChanger(CarWithInteriorConfig config, MaterialHolder matHolder, string[] staticObjectPaths)
        {
            _config = config;
            _materialHolder = matHolder;
            _paths = staticObjectPaths;
        }

        public BasicInteriorChanger(CarWithInteriorConfig config, MaterialHolder matHolder, string staticObjectPath)
            : this(config, matHolder, new[] { staticObjectPath }) { }

        public void Apply(GameObject? interior)
        {
            // Interior unloaded, so don't apply.
            if (interior == null) return;

            _cab = new ChangeObject(interior.transform, IsExploded ? _config.InteriorPrefabExploded : _config.InteriorPrefab,
                _paths.Select(x => interior.transform.Find(x).gameObject).ToArray(), _config.HideOriginalInterior, _materialHolder);

            _config.InteriorApplied(interior, IsExploded);
        }

        public void Unapply(GameObject? interior)
        {
            // Already gone, skip.
            if (interior == null) return;

            _cab?.Clear();

            _config.InteriorUnapplied(interior);
        }
    }
}
