using CarChanger.Common.Configs;
using System.Collections.Generic;
using UnityEngine;

using static CarChanger.Game.IChange;

namespace CarChanger.Game.InteractablesChanges
{
    internal class LocoS282BInteractablesChanger : IInteractablesChanger
    {
        private LocoS282BConfig _config;
        private MaterialHolder _materialHolder;
        private ChangeObject _hatch = null!;
        private ChangeObject _water = null!;
        private IndicatorModelChanger _coal = null!;
        private GameObject[] _ogCoal = null!;
        private float[] _ogPercent = null!;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public LocoS282BInteractablesChanger(LocoS282BConfig config, MaterialHolder matHolder)
        {
            _config = config;
            _materialHolder = matHolder;
        }

        public void Apply(GameObject interactables)
        {
            if (interactables == null) return;

            var hatch = interactables.transform.Find("TenderShaft/C_TenderShaft");
            var water = interactables.transform.Find("Coal&Water/I_TenderWater/tender_water_scaler");

            _hatch = new ChangeObject(hatch, _config.WaterHatch, new[]
                {
                    hatch.transform.Find("model").gameObject
                },
                _config.HideOriginalHatch, _materialHolder);
            _water = new ChangeObject(water, _config.Water, new[]
                {
                    water.transform.Find("tender_water").gameObject
                },
                _config.HideOriginalWater, _materialHolder);

            _coal = interactables.transform.Find("Coal&Water/I_TenderCoal").GetComponent<IndicatorModelChanger>();

            _ogCoal = _coal.indicatorModels;
            _ogPercent = _coal.switchPercentage;

            if (_config.ReplaceCoal)
            {
                var models = new List<GameObject>();

                foreach (var item in _config.CoalModels)
                {
                    var newCoal = Helpers.InstantiateIfNotNull(item, _coal.transform);
                    newCoal.gameObject.SetActive(false);
                    models.Add(newCoal);
                }

                _coal.indicatorModels = models.ToArray();
                _coal.switchPercentage = _config.SwitchPercentage;

                Helpers.RefreshIndicator(_coal);
            }

            _config.InteractablesApplied(interactables, IsExploded);
        }

        public void Unapply(GameObject interactables)
        {
            if (interactables == null) return;

            _hatch.Clear();
            _water.Clear();

            if (_config.ReplaceCoal)
            {
                foreach (var item in _coal.indicatorModels)
                {
                    Object.Destroy(item);
                }

                _coal.indicatorModels = _ogCoal;
                _coal.switchPercentage = _ogPercent;

                Helpers.RefreshIndicator(_coal);
            }

            _config.InteractablesUnapplied(interactables);
        }
    }
}
