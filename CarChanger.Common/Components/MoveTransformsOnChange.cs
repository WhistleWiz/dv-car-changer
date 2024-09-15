using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarChanger.Common.Components
{
    public class MoveTransformsOnChange : MonoBehaviour
    {
        private class OriginalTransform
        {
            public Transform Transform;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;

            public OriginalTransform(Transform transform, Vector3 position, Quaternion rotation, Vector3 scale)
            {
                Transform = transform;
                Position = position;
                Rotation = rotation;
                Scale = scale;
            }

            public OriginalTransform(Transform transform)
                : this(transform, transform.localPosition, transform.localRotation, transform.localScale) { }

            public void Restore()
            {
                if (Transform == null) return;

                Transform.localPosition = Position;
                Transform.localRotation = Rotation;
                Transform.localScale = Scale;
            }
        }

        [Tooltip("Position in local space")]
        public Vector3 Position = Vector3.zero;
        [Tooltip("Rotation in local space")]
        public Vector3 Rotation = Vector3.zero;
        [Tooltip("Scale in local space")]
        public Vector3 Scale = Vector3.one;
        [Tooltip("If true, will apply the transformations relative to their current ones\n" +
            "If not, it will set the corresponding values")]
        public bool Relative = false;

        [Tooltip("The path to the transforms, if on the body it is relative to the car root, " +
            "if on the interactables or interior it's relative to the interior root")]
        public string[] TransformPaths = new string[0];

        private List<OriginalTransform> _transforms = new List<OriginalTransform>();

        private void OnDestroy()
        {
            foreach (OriginalTransform t in _transforms)
            {
                t.Restore();
            }
        }

        public void Apply(Transform root)
        {
            _transforms.Clear();
            var original = TransformPaths.Select(x => root.Find(x)).ToList();

            foreach (Transform t in original)
            {
                if (t == null) continue;

                _transforms.Add(new OriginalTransform(t));

                t.localPosition = Relative ? t.localPosition + Position : Position;
                t.localRotation = Relative ? t.localRotation * Quaternion.Euler(Rotation) : Quaternion.Euler(Rotation);
                t.localScale = Relative ? MultiplyScale(t.localScale, Scale) : Scale;
            }
        }

        private Vector3 MultiplyScale(Vector3 original, Vector3 factor)
        {
            return new Vector3(original.x * factor.x, original.y * factor.y, original.z * factor.z);
        }
    }
}
