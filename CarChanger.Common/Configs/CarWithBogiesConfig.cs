using System.Collections.Generic;
using UnityEngine;

namespace CarChanger.Common.Configs
{
    public abstract class CarWithBogiesConfig : CarConfig
    {
        [Header("Bogies")]
        public bool UseCustomBogies = false;
        [EnableIf(nameof(UseCustomBogies))]
        public float WheelRadius = Constants.Wheels.RadiusDefault;
        [EnableIf(nameof(UseCustomBogies))]
        public GameObject? FrontBogie;
        [EnableIf(nameof(UseCustomBogies))]
        public GameObject? RearBogie;

        [Button(nameof(ResetBogies), "Reset"), SerializeField]
        protected bool ResetBogiesToDefaultButton;

        protected virtual float OriginalRadius => Constants.Wheels.RadiusDefault;

        protected virtual void Reset()
        {
            ResetBogies();
        }

        protected void ResetBogies()
        {
            WheelRadius = OriginalRadius;
            FrontBogie = null;
            RearBogie = null;
        }

        public override IEnumerable<GameObject> GetAllPrefabs(bool includeExploded)
        {
            foreach (var item in base.GetAllPrefabs(includeExploded)) yield return item;
            if (FrontBogie != null) yield return FrontBogie;
            if (RearBogie != null) yield return RearBogie;
        }

        public override bool DoValidation(out string error)
        {
            if (FrontBogie || RearBogie)
            {
                var result = Validation.ValidateBothBogies(FrontBogie, RearBogie);

                if (!string.IsNullOrEmpty(result))
                {
                    error = result!;
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        public static bool CanCombine(CarWithBogiesConfig a, CarWithBogiesConfig b) =>
            CarConfig.CanCombine(a, b) &&
            !(a.UseCustomBogies && b.UseCustomBogies);
    }
}
