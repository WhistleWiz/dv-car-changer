using UnityEngine;

namespace CarChanger.Common.Components
{
    public class SetAsPriorityChild : MonoBehaviour
    {
        private void Awake()
        {
            transform.SetAsFirstSibling();
        }
    }
}
