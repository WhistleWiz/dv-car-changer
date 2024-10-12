using System;
using UnityEngine;

namespace CarChanger.Common.Components
{
    public class ValueToAnimation : MonoBehaviour
    {
        [Tooltip("The animator whose value will be affected")]
        public Animator Animator = null!;
        [Tooltip("The name of the parameter in the animator")]
        public string ParameterName = string.Empty;
        public float Multiplier = 1.0f;
        [Tooltip("The smoothing applied to the value"), Min(0.0f)]
        public float ValueSmoothing = 0.0f;

        public Func<float> ValueGetter = DefaultGetValue;

        private int _id;
        private float _value;
        private float _delta;

        private void Awake()
        {
            if (Animator == null)
            {
                Debug.LogError($"{GetType().Name}: {nameof(Animator)} is null");
                Destroy(this);
                return;
            }

            _id = Animator.StringToHash(ParameterName);
        }

        private void Update()
        {
            if (ValueSmoothing > 0.001f)
            {
                _value = Mathf.SmoothDamp(_value, ValueGetter.Invoke() * Multiplier, ref _delta, ValueSmoothing);
                Animator.SetFloat(_id, _value);
            }
            else
            {
                _value = ValueGetter.Invoke() * Multiplier;
            }

            Animator.SetFloat(_id, _value);
        }

        protected static float DefaultGetValue()
        {
            return 0.0f;
        }
    }
}
