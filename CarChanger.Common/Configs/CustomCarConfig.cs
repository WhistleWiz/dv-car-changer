using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/Custom Car Modification")]
    public class CustomCarConfig : ModelConfig
    {
        [Tooltip("The car type this modification will apply to\n" +
            "Leaving it empty will instead apply to the livery")]
        public string CarTypeId = string.Empty;
        [EnableIf(nameof(EnableLivery)), Tooltip("The livery this modification will apply to")]
        public string LiveryId = string.Empty;

        [Tooltip("The prefab to load on the body")]
        public GameObject BodyPrefab = null!;

        public static bool SameTargets(CustomCarConfig a, CustomCarConfig b)
        {
            if (a.CarTypeId != b.CarTypeId)
            {
                return false;
            }

            if (a.EnableLivery())
            {
                return a.LiveryId == b.LiveryId;
            }

            return true;
        }

        private bool EnableLivery() => string.IsNullOrEmpty(CarTypeId);

        public override bool DoValidation(out string error)
        {
            error = string.Empty;
            return true;
        }
    }
}
