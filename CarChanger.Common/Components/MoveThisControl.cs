using UnityEngine;

namespace CarChanger.Common.Components
{
    [DisallowMultipleComponent]
    public class MoveThisControl : MonoBehaviour
    {
        public Vector3 LocalOffset = Vector3.zero;

        [Space, Tooltip("Changes the axis of the control, if supported")]
        public bool ChangeAxis = false;
        [EnableIf(nameof(ChangeAxis))]
        public Vector3 Axis = Vector3.up;

        [Space, Tooltip("Changes the limits of the control, if supported")]
        public bool ChangeLimits = false;
        [EnableIf(nameof(ChangeLimits))]
        public float Min = 0.0f;
        [EnableIf(nameof(ChangeLimits))]
        public float Max = 90.0f;

        [Space, Tooltip("Replaces the StaticInteractionArea colliders if provided")]
        public GameObject? ReplacementStaticCollider;

        private void OnValidate()
        {
            if (Application.isEditor && transform.parent != null)
            {
                Debug.LogWarning("MoveThisControl component needs to be at the root of the prefab, or it won't be detected!");
            }

            if (Max < Min)
            {
                Max = Min;
            }
        }

        private void OnDrawGizmos()
        {
            if (ChangeAxis)
            {
                Gizmos.DrawLine(transform.position, transform.position + Axis);
            }
        }
    }
}
