using UnityEngine;

namespace CarChanger.Common.Configs
{
    public abstract class CarWithInteriorConfig : CarConfig
    {
        [Header("Interior")]
        [Tooltip("The prefab to load on the interior\n" +
            "Only the static parts of the interior will be affected, all controls will remain as is")]
        public GameObject? InteriorStaticPrefab;
        [Tooltip("The prefab to load on the exploded interior\n" +
            "Only the static parts of the interior will be affected, all controls will remain as is")]
        public GameObject? InteriorStaticPrefabExploded;
        [Tooltip("Whether to hide the original interior or not\n" +
            "Only the static parts of the interior will be affected, all controls will remain as is")]
        public bool HideOriginalInterior = false;

        [Header("Interior LOD")]
        [Tooltip("The prefab to load on the interior LOD\n" +
            "This is the part of the interior shown while the interior itself is unloaded")]
        public GameObject? InteriorLODPrefab;
        [Tooltip("Whether to hide the original interior LOD or not")]
        public bool HideOriginalInteriorLOD = false;

        public static bool CanCombine(CarWithInteriorConfig a, CarWithInteriorConfig b) =>
            CarConfig.CanCombine(a, b) &&
            !(a.HideOriginalInterior && b.HideOriginalInterior) &&
            !(a.HideOriginalInteriorLOD && b.HideOriginalInteriorLOD);
    }
}
