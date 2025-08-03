using UnityEngine;

namespace CarChanger.Common.Components
{
    [AddComponentMenu("Car Changer/Port Value To Rotation")]
    public class PortValueToRotation : ValueToRotation
    {
        [Tooltip("The port ID from the vehicle's simulation")]
        public string PortId = string.Empty;
    }
}
