using System;
using UnityEngine;

namespace CarChanger.Common.Components
{
    public class ValueToRotation : MonoBehaviour
    {
        public Transform Transform = null!;
        public Vector3 Axis = Vector3.up;
        public float RotationSpeed = 1.0f;
        [Min(0.0f)]
        public float ValueSmoothing = 0.0f;

        public Func<float> ValueGetter = DefaultGetValue;

        private float _value;
        private float _delta;

        private void Awake()
        {
            if (Transform == null)
            {
                Debug.LogError($"{GetType().Name}: {nameof(Transform)} is null");
                Destroy(this);
                return;
            }
        }

        private void Update()
        {
            if (ValueSmoothing > 0.001f)
            {
                _value = Mathf.SmoothDamp(_value, ValueGetter.Invoke(), ref _delta, ValueSmoothing);
            }
            else
            {
                _value = ValueGetter.Invoke();
            }

            Transform.Rotate(Axis, _value * RotationSpeed * 360f * Time.deltaTime, Space.Self);
        }

        protected static float DefaultGetValue()
        {
            return 0.0f;
        }
    }
}
