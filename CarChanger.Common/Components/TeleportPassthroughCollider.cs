using UnityEngine;

namespace CarChanger.Common.Components
{
    [AddComponentMenu("Car Changer/Teleport Pass-Through Collider")]
    public class TeleportPassthroughCollider : MonoBehaviour
    {
        public bool TwoSided = true;
        public Collider[] OtherColliders = new Collider[0];
    }
}
