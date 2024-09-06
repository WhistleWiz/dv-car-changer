using UnityEngine;

namespace CarChanger.Common.Configs
{
    public abstract class CarWithInteriorConfig : CarConfig
    {
        [Header("Interior")]
        [Tooltip("The prefab to load on the cab\n" +
            "Only the static parts of the cab will be affected, all controls will remain as is")]
        public GameObject? InteriorStaticPrefab = null;
        [Tooltip("The prefab to load on the exploded cab\n" +
            "Only the static parts of the cab will be affected, all controls will remain as is")]
        public GameObject? InteriorStaticPrefabExploded = null;
        [Tooltip("Whether to hide the original cab or not\n" +
            "Only the static parts of the cab will be affected, all controls will remain as is")]
        public bool HideOriginalInterior = false;

        public static bool CanCombine(CarWithInteriorConfig a, CarWithInteriorConfig b)
        {
            return CarConfig.CanCombine(a, b) &&
                !(a.HideOriginalInterior && b.HideOriginalInterior);
        }
    }
}
