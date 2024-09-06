using UnityEngine;

namespace CarChanger.Common.Configs
{
    public abstract class CarConfig : ModelConfig
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

        public static bool CanCombine(CarConfig a, CarConfig b)
        {
            return !(a.HideOriginalBody && b.HideOriginalBody);
        }
    }
}
