using UnityEngine;

namespace CarChanger.Common.Configs
{
    public abstract class CarConfig : ModelConfig
    {
        [Header("Body")]
        [Tooltip("The prefab to load on the body")]
        public GameObject? BodyPrefab;
        [Tooltip("Whether to hide the original body or not")]
        public bool HideOriginalBody = false;

        [Header("Colliders")]
        [Tooltip("The colliders of the car with the world")]
        public GameObject? CollisionCollider;
        [Tooltip("The colliders of the car with the player")]
        public GameObject? WalkableCollider;
        [Tooltip("The colliders of the car with items")]
        public GameObject? ItemsCollider;

        public static bool CanCombine(CarConfig a, CarConfig b) => !(a.HideOriginalBody && b.HideOriginalBody);
    }
}
