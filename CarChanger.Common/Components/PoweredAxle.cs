using UnityEngine;

namespace CarChanger.Common.Components
{
    [AddComponentMenu("Car Changer/Powered Axle")]
    public class PoweredAxle : MonoBehaviour
    {
        [Tooltip("The rotation axis for the axle")]
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
