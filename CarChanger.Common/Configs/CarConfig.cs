using System.Collections.Generic;
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
        [Tooltip("Whether to prevent other modifications that hide the body from working with this one or not")]
        public bool PreventBodyHiding = false;

        [Header("Buffers")]
        public BufferType BufferType = BufferType.Original;
        public Mesh? CustomBuffer;
        public Material? CustomBufferMaterial;

        [Header("Colliders")]
        [Tooltip("The colliders of the car with the world")]
        public GameObject? CollisionCollider;
        [Tooltip("The colliders of the car with the player")]
        public GameObject? WalkableCollider;
        [Tooltip("The colliders of the car with items")]
        public GameObject? ItemsCollider;

        public bool IncompatibleBodyHiding => HideOriginalBody || PreventBodyHiding;
        public bool ImcompatibleBuffers => BufferType != BufferType.Original;

        public override IEnumerable<GameObject> GetAllPrefabs(bool includeExploded)
        {
            if (BodyPrefab != null) yield return BodyPrefab;
        }

        public static bool CanCombine(CarConfig a, CarConfig b) =>
            !(a.IncompatibleBodyHiding && b.IncompatibleBodyHiding) &&
            !(a.ImcompatibleBuffers && b.ImcompatibleBuffers);
    }
}
