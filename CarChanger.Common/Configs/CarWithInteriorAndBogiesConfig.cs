using UnityEngine;

namespace CarChanger.Common.Configs
{
    public abstract class CarWithInteriorAndBogiesConfig : CarWithInteriorConfig
    {
        [Header("Bogies")]
        public bool UseCustomBogies = false;
        [EnableIf(nameof(EnableBogies))]
        public float WheelRadius = 0.0f;
        [EnableIf(nameof(EnableBogies))]
        public GameObject? FrontBogie = null!;
        [EnableIf(nameof(EnableBogies))]
        public GameObject? RearBogie = null!;

        [Button(nameof(ResetBogies), "Reset"), SerializeField]
        protected bool ResetBogiesToDefaultButton;

        protected virtual float OriginalRadius => Constants.WheelRadiusDefault;

        protected bool EnableBogies() => UseCustomBogies;

        protected void ResetBogies()
        {
            WheelRadius = OriginalRadius;
            FrontBogie = null;
            RearBogie = null;
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

        public static bool CanCombine(CarWithInteriorAndBogiesConfig a, CarWithInteriorAndBogiesConfig b)
        {
            return CarWithInteriorConfig.CanCombine(a, b) &&
                !(a.UseCustomBogies && b.UseCustomBogies);
        }
    }
}
