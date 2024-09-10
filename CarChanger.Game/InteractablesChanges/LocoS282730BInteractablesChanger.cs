using CarChanger.Common.Configs;
using UnityEngine;

namespace CarChanger.Game.InteractablesChanges
{
    internal class LocoS282730BInteractablesChanger : IInteractablesChanger
    {
        private LocoS282730BConfig _config;
        private MaterialHolder _materialHolder;
        private ChangeObject? _hatch;
        private ChangeObject? _water;
        private IndicatorModelChangerHelper? _coal;

        private bool IsExploded => _materialHolder.Car.isExploded;

        public LocoS282730BInteractablesChanger(LocoS282730BConfig config, MaterialHolder matHolder)
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

            if (_config.ReplaceCoal)
            {
                _coal = new IndicatorModelChangerHelper(
                    interactables.transform.Find("Coal&Water/I_TenderCoal").GetComponent<IndicatorModelChanger>(),
                    _config.CoalModels,
                    _config.SwitchPercentage);
            }

            _config.InteractablesApplied(interactables, IsExploded);
        }

        public void Unapply(GameObject interactables)
        {
            if (interactables == null) return;

            _hatch?.Clear();
            _water?.Clear();
            _coal?.Clear();

            _config.InteractablesUnapplied(interactables);
        }
    }
}
