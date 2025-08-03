using UnityEngine;

namespace CarChanger.Common.Components
{
    [AddComponentMenu("Car Changer/Port Value To Animation")]
    public class PortValueToAnimation : ValueToAnimation
    {
        [Tooltip("The port ID from the vehicle's simulation")]
        public string PortId = string.Empty;
    }
}
