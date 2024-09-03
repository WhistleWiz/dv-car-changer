using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.InteriorChanges
{
    internal class LocoDE6InteriorChanger : IInteriorChanger
    {
        private LocoDE6Config _config;
        private MaterialHolder _materialHolder;
        private GameObject _instanced = null!;
        private GameObject _cab = null!;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public LocoDE6InteriorChanger(LocoDE6Config config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject interior)
        {
            // Interior unloaded, so don't apply.
            if (interior == null) return;
            
            _instanced = Helpers.InstantiateIfNotNull(IsExploded ? _config.CabStaticPrefabExploded : _config.CabStaticPrefab, interior.transform);

            _cab = interior.transform.Find("Cab").gameObject;

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
