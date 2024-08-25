using UnityEngine;

namespace CarChanger.Common.Components
{
    public class RandomisePosition : MonoBehaviour
    {
        public Vector3 PositionRange;
        public Vector3 RotationRange;

        private void Start()
        {
            Vector3 posOffset = new Vector3(Random.Range(-PositionRange.x, PositionRange.x),
                Random.Range(-PositionRange.y, PositionRange.y),
                Random.Range(-PositionRange.z, PositionRange.z));

            Vector3 rotOffset = new Vector3(Random.Range(-RotationRange.x, RotationRange.x),
                Random.Range(-RotationRange.y, RotationRange.y),
                Random.Range(-RotationRange.z, RotationRange.z));

            transform.localPosition += posOffset;
            transform.localRotation *= Quaternion.Euler(rotOffset);
        }
    }
}
