using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/Passenger Coach Modification", order = Constants.MenuOrderConstants.Unpowered + 1)]
    public class PassengerConfig : ModelConfig
    {
        // Passenger gets a separate one from the rest of the wagons
        // to allow changing the interior.
        [Header("Body")]
        [Tooltip("The coach liveries this modification will apply to")]
        public PassengerType Livery = PassengerType.All;
        [Tooltip("The prefab to load on the body")]
        public GameObject BodyPrefab = null!;
        [Tooltip("Whether to hide the original body or not")]
        public bool HideOriginalBody = false;
        [Tooltip("Whether to hide the original interior or not")]
        public bool HideOriginalInterior = false;

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

        public static bool CanCombine(PassengerConfig a, PassengerConfig b)
        {
            return !(a.UseCustomBogies && b.UseCustomBogies) &&
                !(a.HideOriginalBody && b.HideOriginalBody) &&
                !(a.HideOriginalInterior && b.HideOriginalInterior);
        }

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
