using CarChanger.Common.Components;
using DV.CabControls.Spec;
using System.Collections;
using UnityEngine;

namespace CarChanger.Game.Components
{
    internal class MoveThisControlInternal : MonoBehaviour
    {
        private ControlSpec _control = null!;
        private Vector3 _localOffset = Vector3.zero;
        private bool _moved = false;

        private IEnumerator Start()
        {
            Joint joint;

            // Wait for the joint to get created.
            while (!_control.TryGetComponent(out joint))
            {
                yield return null;
            }

            Helpers.MoveControl(_control, joint, _localOffset);
            _moved = true;
        }

        private void OnDestroy()
        {
            if (_moved)
            {
                Helpers.MoveControl(_control, -_localOffset);
            }
        }

        public static MoveThisControlInternal Create(MoveThisControl comp, ControlSpec control)
        {
            var real = comp.gameObject.AddComponent<MoveThisControlInternal>();
            real._control = control;
            real._localOffset = comp.LocalOffset;
            return real;
        }
    }
}
