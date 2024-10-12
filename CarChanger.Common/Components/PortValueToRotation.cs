using UnityEngine;

namespace CarChanger.Common.Components
{
    public class PortValueToRotation : ValueToRotation
    {
        [Tooltip("The port ID from the vehicle's simulation")]
        public string PortId = string.Empty;
    }
}
