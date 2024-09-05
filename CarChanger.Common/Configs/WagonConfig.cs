using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/Wagon Modification", order = Constants.MenuOrderConstants.Unpowered + 0)]
    public class WagonConfig : ModelConfig
    {
        [Header("Body")]
        [Tooltip("The car type this modification will apply to\n" +
            "'Use Livery' applies it to a single livery instead of all liveries of a type")]
        public WagonType CarType = WagonType.Flatbed;
        [EnableIf(nameof(EnableLivery)), Tooltip("The livery this modification will apply to")]
        public WagonLivery CarLivery = WagonLivery.FlatbedEmpty;
        [Tooltip("The prefab to load on the body")]
        public GameObject BodyPrefab = null!;
        [Tooltip("Whether to hide the original body or not")]
        public bool HideOriginalBody = false;

        [Header("Colliders")]
        [Tooltip("The colliders of the car with the world")]
        public GameObject? CollisionCollider = null;
        [Tooltip("The colliders of the car with the player")]
        public GameObject? WalkableCollider = null;
        [Tooltip("The colliders of the car with items")]
        public GameObject? ItemsCollider = null;

        [Header("Bogies")]
        public bool UseCustomBogies = false;
        [EnableIf(nameof(EnableBogies))]
        public float WheelRadius = Constants.WheelRadiusDefault;
        [EnableIf(nameof(EnableBogies))]
        public GameObject? FrontBogie = null;
        [EnableIf(nameof(EnableBogies))]
        public GameObject? RearBogie = null;
        [Button(nameof(ResetBogies), "Reset"), SerializeField]
        private bool _resetBogiesToDefaultButton;

        public static bool CanCombine(WagonConfig a, WagonConfig b)
        {
            return !(a.UseCustomBogies && b.UseCustomBogies) &&
                !(a.HideOriginalBody && b.HideOriginalBody);
        }

        public static bool SameTargets(WagonConfig a, WagonConfig b)
        {
            if (a.CarType != b.CarType)
            {
                return false;
            }

            if (a.CarType == WagonType.UseLivery)
            {
                return a.CarLivery == b.CarLivery;
            }

            return true;
        }

        private bool EnableLivery() => CarType == WagonType.UseLivery;

        private bool EnableBogies() => UseCustomBogies;

        private void ResetBogies()
        {
            WheelRadius = Constants.WheelRadiusDefault;
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
    }
}
