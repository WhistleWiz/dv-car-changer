using System;
using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/S282A Modification", order = Constants.MenuOrderConstants.Steam + 1)]
    public class LocoS282AConfig : LocomotiveConfig
    {
        private static readonly Vector3 OriginalBeamPosition = new Vector3(0.0f, 3.579122f, 10.98136f);

        [Header("Headlights")]
        public bool UseCustomHeadlights = false;
        [EnableIf(nameof(EnableHeadlights))]
        public Mesh Mesh = null!;
        [EnableIf(nameof(EnableHeadlights))]
        public Vector3 BeamPosition = OriginalBeamPosition;
        [Button(nameof(ResetHeadlights), "Reset Position"), SerializeField]
        private bool _resetHeadlight;

        /// <summary>
        /// Called when this config is applied to the interior. This may happen multiple times while the outside is only applied once,
        /// as the interior loads and unloads. Receives itself, the interior <see cref="GameObject"/> of a car, and whether or not it is exploded.
        /// </summary>
        public event Action<LocoS282AConfig, GameObject, bool>? OnInteriorApplied;
        /// <summary>
        /// Called when this config is unapplied to the interior. This is only called when the interior is active and the config is unapplied.
        /// It will not be called when unloading the interior. Receives itself and the interior <see cref="GameObject"/> of a car.
        /// </summary>
        public event Action<LocoS282AConfig, GameObject>? OnInteriorUnapplied;

        /// <summary>
        /// Called when this config is applied to the external interactables.
        /// Receives itself, the interactables <see cref="GameObject"/> of a car, and whether or not it is exploded.
        /// </summary>
        public event Action<LocoS282AConfig, GameObject, bool>? OnInteractablesApplied;
        /// <summary>
        /// Called when this config is unapplied to the external interactables.
        /// Receives itself and the interactables <see cref="GameObject"/> of a car.
        /// </summary>
        public event Action<LocoS282AConfig, GameObject>? OnInteractablesUnapplied;

        public override bool DoValidation(out string error)
        {
            error = string.Empty;
            return true;
        }

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

        public void ResetHeadlights()
        {
            BeamPosition = OriginalBeamPosition;
        }

        private bool EnableHeadlights() => UseCustomHeadlights;

        public static bool CanCombine(LocoS282AConfig a, LocoS282AConfig b)
        {
            return LocomotiveConfig.CanCombine(a, b);
        }
    }
}
