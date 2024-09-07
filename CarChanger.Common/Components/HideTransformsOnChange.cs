using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarChanger.Common.Components
{
    public class HideTransformsOnChange : MonoBehaviour
    {
        public string[] TransformPaths = new string[0];

        private List<Transform> _transforms = new List<Transform>();

        private void OnDestroy()
        {
            foreach (Transform t in _transforms)
            {
                if (t == null) continue;

                t.gameObject.SetActive(true);
            }
        }

        public void Hide(Transform root)
        {
            _transforms.Clear();
            _transforms = TransformPaths.Select(x => root.Find(x)).ToList();

            foreach (Transform t in _transforms)
            {
                if (t == null) continue;

                t.gameObject.SetActive(false);
            }
        }
    }
}
