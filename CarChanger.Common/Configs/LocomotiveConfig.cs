using System;
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

        /// <summary>
        /// Called when this config is applied to the interior. This may happen multiple times while the outside is only applied once,
        /// as the interior loads and unloads. Receives itself, the interior <see cref="GameObject"/> of a car, and whether or not it is exploded.
        /// </summary>
        public event Action<LocomotiveConfig, GameObject, bool>? OnInteriorApplied;
        /// <summary>
        /// Called when this config is unapplied to the interior. This is only called when the interior is active and the config is unapplied.
        /// It will not be called when unloading the interior. Receives itself and the interior <see cref="GameObject"/> of a car.
        /// </summary>
        public event Action<LocomotiveConfig, GameObject>? OnInteriorUnapplied;

        /// <summary>
        /// Called when this config is applied to the external interactables.
        /// Receives itself, the interactables <see cref="GameObject"/> of a car, and whether or not it is exploded.
        /// </summary>
        public event Action<LocomotiveConfig, GameObject, bool>? OnInteractablesApplied;
        /// <summary>
        /// Called when this config is unapplied to the external interactables.
        /// Receives itself and the interactables <see cref="GameObject"/> of a car.
        /// </summary>
        public event Action<LocomotiveConfig, GameObject>? OnInteractablesUnapplied;

        public void InteriorApplied(GameObject gameObject, bool isExploded)
        {
            OnInteriorApplied?.Invoke(this, gameObject, isExploded);
        }

        public void InteriorUnapplied(GameObject gameObject)
        {
            OnInteriorUnapplied?.Invoke(this, gameObject);
        }

        public void InteractablesApplied(GameObject gameObject, bool isExploded)
        {
            OnInteractablesApplied?.Invoke(this, gameObject, isExploded);
        }

        public void InteractablesUnapplied(GameObject gameObject)
        {
            OnInteractablesUnapplied?.Invoke(this, gameObject);
        }

        public static bool CanCombine(LocomotiveConfig a, LocomotiveConfig b)
        {
            return !(a.HideOriginalBody && b.HideOriginalBody) &&
                !(a.HideOriginalCab && b.HideOriginalCab);
        }
    }
}
