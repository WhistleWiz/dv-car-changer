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

        public Func<float> GetValue = DefaultGetValue;

        private int _id;

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
            Animator.SetFloat(_id, GetValue.Invoke() * Multiplier);
        }

        protected static float DefaultGetValue()
        {
            return 0.0f;
        }
    }
}
