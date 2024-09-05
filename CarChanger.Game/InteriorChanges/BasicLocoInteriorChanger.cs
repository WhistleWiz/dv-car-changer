using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.InteriorChanges
{
    internal class BasicLocoInteriorChanger : IInteriorChanger
    {
        private LocomotiveConfig _config;
        private MaterialHolder _materialHolder;
        private GameObject _instanced = null!;
        private GameObject _cab = null!;
        private string _path;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public BasicLocoInteriorChanger(LocomotiveConfig config, MaterialHolder matHolder, string staticObjectPath)
        {
            _config = config;
            _materialHolder = matHolder;
            _path = staticObjectPath;

        }

        public void Apply(GameObject interior)
        {
            // Interior unloaded, so don't apply.
            if (interior == null) return;

            _instanced = Helpers.InstantiateIfNotNull(IsExploded ? _config.CabStaticPrefabExploded : _config.CabStaticPrefab, interior.transform);

            _cab = interior.transform.Find(_path).gameObject;

            if (_config.HideOriginalCab)
            {
                _cab.SetActive(false);
            }

            ComponentProcessor.ProcessComponents(_instanced, _materialHolder);

            _config.InteriorApplied(interior, IsExploded);
        }

        public void Unapply(GameObject interior)
        {
            // Already gone, skip.
            if (interior == null) return;

            Helpers.DestroyIfNotNull(_instanced);

            if (_config.HideOriginalCab && _cab != null)
            {
                _cab.SetActive(true);
            }

            _config.InteriorUnapplied(interior);

            _instanced = null!;
            _cab = null!;
        }
    }
}
