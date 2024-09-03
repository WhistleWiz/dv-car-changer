using UnityEngine;

namespace CarChanger.Common.Components
{
    public class TeleportPassthroughCollider : MonoBehaviour
    {
        public bool TwoSided = true;
        public Collider[] OtherColliders = new Collider[0];
    }
}
