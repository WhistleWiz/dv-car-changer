using System.Collections.Generic;
using UnityEngine;

namespace CarChanger.Common.Configs
{
    public abstract class CarWithInteriorConfig : CarConfig
    {
        [Header("Interior")]
        [Tooltip("The prefab to load on the interior\n" +
            "Only the static parts of the interior will be affected, all controls will remain as is")]
        public GameObject? InteriorPrefab;
        [Tooltip("The prefab to load on the exploded interior\n" +
            "Only the static parts of the interior will be affected, all controls will remain as is"),
            EnableIf(nameof(CanExplode))]
        public GameObject? InteriorPrefabExploded;
        [Tooltip("Whether to hide the original interior or not\n" +
            "Only the static parts of the interior will be affected, all controls will remain as is")]
        public bool HideOriginalInterior = false;
        [Tooltip("Whether to prevent other modifications that hide the interior from working with this one or not")]
        public bool PreventInteriorHiding = false;

        [Header("Interior LOD")]
        [Tooltip("The prefab to load on the interior LOD\n" +
            "This is the part of the interior shown while the interior itself is unloaded")]
        public GameObject? InteriorLODPrefab;
        [Tooltip("Whether to hide the original interior LOD or not")]
        public bool HideOriginalInteriorLOD = false;
        [Tooltip("Whether to prevent other modifications that hide the interior LOD from working with this one or not")]
        public bool PreventInteriorLODHiding = false;

        public bool IncompatibleInteriorHiding => HideOriginalInterior || PreventInteriorHiding;
        public bool IncompatibleInteriorLODHiding => HideOriginalInteriorLOD || PreventInteriorLODHiding;

        public bool CanExplode() => CanExplode(this);

        public override IEnumerable<GameObject> GetAllPrefabs(bool includeExploded)
        {
            foreach (var item in base.GetAllPrefabs(includeExploded)) yield return item;
            if (InteriorLODPrefab != null) yield return InteriorLODPrefab;
            if (InteriorPrefab != null) yield return InteriorPrefab;
            if (includeExploded && InteriorPrefabExploded != null) yield return InteriorPrefabExploded;
        }

        public static bool CanCombine(CarWithInteriorConfig a, CarWithInteriorConfig b) =>
            CarConfig.CanCombine(a, b) &&
            !(a.IncompatibleInteriorHiding && b.IncompatibleInteriorHiding) &&
            !(a.IncompatibleInteriorLODHiding && b.IncompatibleInteriorLODHiding);

        public static bool CanExplode(CarConfig config) => config switch
        {
            CabooseConfig _ => false,
            LocoDM1U150Config _ => false,
            _ => true,
        };
    }
}
