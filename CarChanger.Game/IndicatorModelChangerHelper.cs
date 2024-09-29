using System.Collections.Generic;
using UnityEngine;

namespace CarChanger.Game
{
    internal class IndicatorModelChangerHelper
    {
        private IndicatorModelChanger _indicator;
        private GameObject[] _models;
        private float[] _percents;

        public IndicatorModelChangerHelper(IndicatorModelChanger indicator, GameObject[] models, float[] percents)
        {
            _indicator = indicator;
            _models = _indicator.indicatorModels;
            _percents = _indicator.switchPercentage;

            List<GameObject> newModels = new List<GameObject>();

            foreach (GameObject model in models)
            {
                var go = Helpers.InstantiateIfNotNull(model, indicator.transform);
                go.gameObject.SetActive(false);
                newModels.Add(go);
            }

            foreach (GameObject model in _models)
            {
                model.SetActive(false);
            }

            _indicator.indicatorModels = newModels.ToArray();
            _indicator.switchPercentage = percents;

            Helpers.RefreshIndicator(_indicator);
        }

        public void Clear()
        {
            if (_indicator == null) return;

            foreach (var item in _indicator.indicatorModels)
            {
                Object.Destroy(item);
            }

            _indicator.indicatorModels = _models;
            _indicator.switchPercentage = _percents;

            Helpers.RefreshIndicator(_indicator);
        }
    }
}
