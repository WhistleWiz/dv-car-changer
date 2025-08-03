using System;
using UnityEngine;

namespace CarChanger.Common.Components
{
    [AddComponentMenu("Car Changer/Value To Rotation")]
    public class ValueToRotation : MonoBehaviour
    {
        public Transform[] Transforms = new Transform[0];
        public Vector3 Axis = Vector3.up;
        public float RotationsPerSecond = 1.0f;
        [Min(0.0f)]
        public float ValueSmoothing = 0.0f;

        public Func<float> ValueGetter = DefaultGetValue;

        private float _value = 0.0f;
        private float _delta = 0.0f;

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

            foreach (var rotation in Transforms)
            {
                rotation.Rotate(Axis, _value * RotationsPerSecond * 360f * Time.deltaTime, Space.Self);
            }
        }

        protected static float DefaultGetValue() => 0.0f;
    }
}
