using UnityEngine;

namespace CarChanger.Common.Components
{
    [AddComponentMenu("Car Changer/Set As Priority Child")]
    public class SetAsPriorityChild : MonoBehaviour
    {
        private void Awake()
        {
            transform.SetAsFirstSibling();
        }
    }
}
