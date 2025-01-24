using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarChanger.Common.Components
{
    public class DuplicateTransformsOnChange : MonoBehaviour
    {
        private const char Slash = '/';

        [Tooltip("The path to the transforms, if on the body it is relative to the car root, " +
            "if on the interactables or interior it's relative to the interior root")]
        public string[] TransformPaths = new string[0];
        [Tooltip("Whether to keep the same parents for the dupe or place them at the root")]
        public bool KeepParent = true;
        [Tooltip("This ID is appended to the original transform name so it can be easily identified " +
            "by other components (i.e. chaining duplicate with move)\n" +
            "Leaving this empty appends '(Clone)' as usual")]
        public string DuplicationId = string.Empty;

        private List<Transform> _transforms = new List<Transform>();
        private List<Transform> _duplicates = new List<Transform>();

        private void OnValidate()
        {
            if (DuplicationId.Contains(Slash))
            {
                Debug.LogError($"Cannot use '{Slash}' in IDs!");
                DuplicationId = string.Join(string.Empty, DuplicationId.Split(Slash));
            }
        }

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

                var dupe = Instantiate(t, KeepParent ? t.parent : root);

                if (!string.IsNullOrEmpty(DuplicationId))
                {
                    dupe.name = t.name + DuplicationId;
                }

                _duplicates.Add(dupe);
            }
        }
    }
}
