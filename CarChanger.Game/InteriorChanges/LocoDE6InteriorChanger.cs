using CarChanger.Common.Configs;
using UnityEngine;
using static CarChanger.Game.AppliedChange;

namespace CarChanger.Game.InteriorChanges
{
    internal class LocoDE6InteriorChanger : IInteriorChanger
    {
        private LocoDE6Config _config;
        private MaterialHolder _materialHolder;
        private GameObject _interior = null!;
        private GameObject _instanced = null!;
        private GameObject _cab = null!;

        public LocoDE6InteriorChanger(LocoDE6Config config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
            matHolder.Car.InteriorLoaded += Apply;
        }

        public void Apply(GameObject interior)
        {
            _interior = interior;
            _instanced = Object.Instantiate(_config.CabStaticPrefab, interior.transform);
            _cab = interior.transform.Find("Cab").gameObject;

            if (_config.HideOriginalCab)
            {
                _cab.SetActive(false);
            }

            ProcessComponents(_instanced, _materialHolder);

            _config.InteriorApplied(interior);
        }

        public void Unapply()
        {
            if (_instanced != null)
            {
                Object.Destroy(_instanced);
            }

            if (_config.HideOriginalCab)
            {
                _cab.SetActive(true);
            }

            _config.InteriorUnapplied(_interior);
        }
    }
}
