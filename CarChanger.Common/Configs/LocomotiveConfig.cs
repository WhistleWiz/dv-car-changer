using UnityEngine;

namespace CarChanger.Common.Configs
{
    public abstract class LocomotiveConfig : ModelConfig
    {
        [Header("Body")]
        [Tooltip("The prefab to load on the body")]
        public GameObject? BodyPrefab = null;
        [Tooltip("Whether to hide the original body or not")]
        public bool HideOriginalBody = false;

        [Header("Colliders")]
        [Tooltip("The colliders of the car with the world")]
        public GameObject? CollisionCollider = null;
        [Tooltip("The colliders of the car with the player")]
        public GameObject? WalkableCollider = null;
        [Tooltip("The colliders of the car with items")]
        public GameObject? ItemsCollider = null;

        [Header("Interior")]
        [Tooltip("The prefab to load on the cab\n" +
            "Only the static parts of the cab will be affected, all controls will remain as is")]
        public GameObject? CabStaticPrefab = null;
        [Tooltip("The prefab to load on the exploded cab\n" +
            "Only the static parts of the cab will be affected, all controls will remain as is")]
        public GameObject? CabStaticPrefabExploded = null;
        [Tooltip("Whether to hide the original cab or not\n" +
            "Only the static parts of the cab will be affected, all controls will remain as is")]
        public bool HideOriginalCab = false;

        public static bool CanCombine(LocomotiveConfig a, LocomotiveConfig b)
        {
            return !(a.HideOriginalBody && b.HideOriginalBody) &&
                !(a.HideOriginalCab && b.HideOriginalCab);
        }
    }
}
