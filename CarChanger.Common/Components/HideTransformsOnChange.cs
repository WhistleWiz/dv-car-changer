using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarChanger.Common.Components
{
    [AddComponentMenu("Car Changer/Hide Transforms On Change")]
    public class HideTransformsOnChange : MonoBehaviour
    {
        [Tooltip("The path to the transforms, if on the body it is relative to the car root, " +
            "if on the interactables or interior it's relative to the interior root")]
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
