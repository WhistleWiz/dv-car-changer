using UnityEngine;

namespace CarChanger.Common.Components
{
    [AddComponentMenu("Car Changer/Randomise Position")]
    public class RandomisePosition : MonoBehaviour
    {
        public Vector3 PositionRange;
        public Vector3 RotationRange;

        public Vector3 ActualPositionRange => PositionRange * 0.5f;
        public Vector3 ActualRotationRange => RotationRange * 0.5f;

        private void Start()
        {
            var posRange = ActualPositionRange;
            Vector3 posOffset = new Vector3(Random.Range(-posRange.x, posRange.x),
                Random.Range(-posRange.y, posRange.y),
                Random.Range(-posRange.z, posRange.z));

            var rotRange = ActualRotationRange;
            Vector3 rotOffset = new Vector3(Random.Range(-rotRange.x, rotRange.x),
                Random.Range(-rotRange.y, rotRange.y),
                Random.Range(-rotRange.z, rotRange.z));

            transform.localPosition += posOffset;
            transform.localRotation *= Quaternion.Euler(rotOffset);
        }
    }
}
