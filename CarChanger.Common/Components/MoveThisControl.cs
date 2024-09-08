using UnityEngine;

namespace CarChanger.Common.Components
{
    [DisallowMultipleComponent]
    public class MoveThisControl : MonoBehaviour
    {
        public Vector3 LocalOffset = Vector3.zero;

        private void OnValidate()
        {
            if (Application.isEditor && transform.parent != null)
            {
                Debug.LogWarning("MoveThisControl component needs to be at the root of the prefab, or it won't be detected!");
            }
        }
    }
}
