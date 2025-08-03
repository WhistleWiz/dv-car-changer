using UnityEngine;

namespace CarChanger.Common.Components
{
    [AddComponentMenu("Car Changer/Serviceable Collider")]
    public class ServiceableCollider : MonoBehaviour
    {
        private void Start()
        {
            tag = "MainTriggerCollider";
        }
    }
}
