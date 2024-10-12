using System;
using UnityEngine;

namespace CarChanger.Common.Components
{
    public class PortValueToAnimation : ValueToAnimation
    {
        [Tooltip("The port ID from the vehicle's simulation")]
        public string PortId = string.Empty;
    }
}
