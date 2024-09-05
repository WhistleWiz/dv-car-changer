using UnityEngine;

namespace CarChanger.Common.Configs
{
    public abstract class LocomotiveWithBogiesConfig : LocomotiveConfig
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

        protected abstract float OriginalRadius { get; }

        protected bool EnableBogies() => UseCustomBogies;

        protected void ResetBogies()
        {
            WheelRadius = OriginalRadius;
            FrontBogie = null;
            RearBogie = null;
        }

        public static bool CanCombine(LocomotiveWithBogiesConfig a, LocomotiveWithBogiesConfig b)
        {
            return LocomotiveConfig.CanCombine(a, b) &&
                !(a.UseCustomBogies && b.UseCustomBogies);
        }
    }
}
