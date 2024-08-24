using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/Wagon Modification")]
    public class WagonConfig : ModelConfig
    {
        [Header("Wagon Settings")]
        [Tooltip("The car type this modification will apply to\n" +
            "'Use Livery' applies it to a single livery instead of all liveries of a type")]
        public WagonType CarType = WagonType.Flatbed;
        [EnableIf(nameof(EnableLivery)), Tooltip("The livery this modification will apply to")]
        public WagonLivery CarLivery = WagonLivery.FlatbedEmpty;
        [Tooltip("The prefab to load on the body")]
        public GameObject BodyPrefab = null!;
        [Tooltip("Whether to hide the original body or not")]
        public bool HideOriginalBody = false;

        [Header("Bogies")]
        public bool UseCustomBogies = false;
        [EnableIf(nameof(EnableBogies))]
        public float WheelRadius = 0.459f;
        [EnableIf(nameof(EnableBogies))]
        public GameObject FrontBogie = null!;
        [EnableIf(nameof(EnableBogies))]
        public GameObject RearBogie = null!;

        public static bool CanCombine(WagonConfig a, WagonConfig b)
        {
            return !(a.UseCustomBogies && b.UseCustomBogies) &&
                !(a.HideOriginalBody && b.HideOriginalBody);
        }

        private bool EnableLivery() => CarType == WagonType.UseLivery;

        private bool EnableBogies() => UseCustomBogies;

        public override bool DoValidation(out string error)
        {
            error = string.Empty;
            return true;
        }
    }
}
