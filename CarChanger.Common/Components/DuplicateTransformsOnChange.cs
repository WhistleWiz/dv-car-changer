using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarChanger.Common.Components
{
    public class DuplicateTransformsOnChange : MonoBehaviour
    {
        [Tooltip("The path to the transforms, if on the body it is relative to the car root, " +
            "if on the interactables or interior it's relative to the interior root")]
        public string[] TransformPaths = new string[0];
        [Tooltip("Whether to keep the same parents for the dupe or place them at the root")]
        public bool KeepParent = true;

        private List<Transform> _transforms = new List<Transform>();
        private List<Transform> _duplicates = new List<Transform>();

        private void OnDestroy()
        {
            foreach (Transform t in _duplicates)
            {
                if (t == null) continue;

                Destroy(t);
            }
        }

        public void Duplicate(Transform root)
        {
            _transforms.Clear();
            _transforms = TransformPaths.Select(x => root.Find(x)).ToList();

            foreach (Transform t in _transforms)
            {
                if (t == null) continue;

                _duplicates.Add(Instantiate(t, KeepParent ? t.parent : root));
            }
        }
    }
}
