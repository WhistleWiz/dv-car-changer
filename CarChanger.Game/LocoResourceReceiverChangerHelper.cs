using UnityEngine;

namespace CarChanger.Game
{
    internal class LocoResourceReceiverHelper
    {
        private LocoResourceReceiver _original;
        private GameObject _newReceiver;
        private GameObject? _blocker;

        public LocoResourceReceiverHelper(LocoResourceReceiver original, GameObject newReceiver, GameObject? blocker = null)
        {
            var type = original.resourceType;

            _original = original;
            _newReceiver = Object.Instantiate(newReceiver, original.transform.parent);
            _blocker = blocker;

            GameObject temp;

            foreach (var collider in _newReceiver.GetComponentsInChildren<Collider>())
            {
                temp = collider.gameObject;

                if (temp.TryGetComponent(out LocoResourceReceiver receiver)) continue;

                temp.AddComponent<InteriorNonStandardLayer>();
                temp.tag = original.gameObject.tag;
                temp.layer = original.gameObject.layer;

                receiver = temp.AddComponent<LocoResourceReceiver>();
                receiver.resourceType = type;
            }

            _original.gameObject.SetActive(false);
            _blocker?.SetActive(false);
        }

        public void Clear()
        {
            Object.Destroy(_newReceiver);
            _original.gameObject.SetActive(true);
            _blocker?.SetActive(true);
        }
    }
}
