using UnityEngine;

namespace CarChanger.Common.Components
{
    [AddComponentMenu("Car Changer/Swap GameObject On Explosion")]
    public class SwapGameObjectOnExplosion : MonoBehaviour
    {
        public GameObject ReplacePrefab = null!;
    }
}
