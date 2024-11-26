using CarChanger.Common.Configs;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game.InteriorChanges
{
    internal class BasicInteriorChanger : IInteriorChanger
    {
        private CarWithInteriorConfig _config;
        private ChangeObject? _cab;
        private string[] _paths;

        protected MaterialHolder MaterialHolder;

        private bool IsExploded => MaterialHolder.Car.isExploded;

        public BasicInteriorChanger(CarWithInteriorConfig config, MaterialHolder matHolder, string[] staticObjectPaths)
        {
            _config = config;
            _paths = staticObjectPaths;
            MaterialHolder = matHolder;
        }

        public BasicInteriorChanger(CarWithInteriorConfig config, MaterialHolder matHolder, string staticObjectPath)
            : this(config, matHolder, new[] { staticObjectPath }) { }

        public BasicInteriorChanger(CarWithInteriorConfig config, MaterialHolder matHolder, params (string Path, bool Use)[] staticObjectPaths)
            : this(config, matHolder, staticObjectPaths.Where(x => x.Use).Select(x => x.Path).ToArray()) { }

        public virtual void Apply(GameObject? interior)
        {
            // Interior unloaded, so don't apply.
            if (interior == null) return;

            _cab = new ChangeObject(interior.transform, IsExploded ? _config.InteriorPrefabExploded : _config.InteriorPrefab,
                _paths.Select(x => interior.transform.Find(x).gameObject).ToArray(), _config.HideOriginalInterior, MaterialHolder);

            _config.InteriorApplied(interior, IsExploded);
        }

        public virtual void Unapply(GameObject? interior)
        {
            // Already gone, skip.
            if (interior == null) return;

            _cab?.Clear();

            _config.InteriorUnapplied(interior);
        }
    }
}
