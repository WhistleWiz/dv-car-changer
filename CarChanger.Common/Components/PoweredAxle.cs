using UnityEngine;

namespace CarChanger.Common.Components
{
    public class PoweredAxle : MonoBehaviour
    {
        public Vector3 Axis = Vector3.right;

        private void Reset()
        {
            name = Constants.Axle;
        }

        private void OnValidate()
        {
            name = Constants.Axle;
        }
    }
}
