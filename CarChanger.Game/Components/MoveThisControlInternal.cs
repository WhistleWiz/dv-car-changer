using CarChanger.Common.Components;
using DV.CabControls.Spec;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarChanger.Game.Components
{
    internal class MoveThisControlInternal : MonoBehaviour
    {
        // Prevent an infinite loop.
        private const int Safety = 1024;

        private ControlSpec _control = null!;
        private Vector3 _localOffset = Vector3.zero;
        private Vector2? _limits;
        private Vector3? _axis;
        private GameObject? _replaceCollider;
        private List<GameObject> _disabledColliders = new List<GameObject>();

        private bool _moved = false;
        private Vector2? _ogLimits;
        private Vector3? _ogAxis;

        private IEnumerator Start()
        {
            Joint joint;
            int counter = Safety;

            // Wait for the joint to get created.
            while (!_control.TryGetComponent(out joint))
            {
                if (counter-- < 0) yield break;

                yield return null;
            }

            // Move the joint.
            Helpers.MoveControl(_control, joint, _localOffset);

            // Change the joint axis if supplied.
            if (_axis.HasValue)
            {
                _ogAxis = joint.axis;
                joint.axis = _axis.Value;
            }

            // Change the limits if supplied and the joint is an HingeJoint.
            if (_limits.HasValue && joint is HingeJoint hinge && hinge.useLimits)
            {
                var limits = hinge.limits;
                _ogLimits = new Vector2(limits.min, limits.max);
                limits.min = _limits.Value.x;
                limits.max = _limits.Value.y;
                hinge.limits = limits;
            }

            // Change the collider of the StaticInteractionArea if possible.
            if (_replaceCollider != null)
            {
                var ia = Helpers.GetInteractionArea(_control);

                if (ia != null)
                {
                    foreach (var item in ia.GetComponentsInChildren<Collider>(true))
                    {
                        if (!item.gameObject.activeSelf) continue;

                        item.gameObject.SetActive(false);
                        _disabledColliders.Add(item.gameObject);
                    }

                    _replaceCollider.transform.SetParent(ia.transform, false);
                    _replaceCollider.SetLayersRecursive(ia.gameObject.layer);
                }
                else
                {
                    // Clean out the colliders if they are not supported.
                    Destroy(_replaceCollider);
                }
            }

            _moved = true;
        }

        private void OnDestroy()
        {
            if (!_moved) return;

            Helpers.MoveControl(_control, -_localOffset);
            var joint = _control.GetComponent<Joint>();

            if (_replaceCollider != null)
            {
                Destroy(_replaceCollider);
            }

            foreach (var item in _disabledColliders)
            {
                item.gameObject.SetActive(true);
            }

            if (joint != null)
            {
                if (_ogAxis.HasValue)
                {
                    joint.axis = _ogAxis.Value;
                }

                if (_ogLimits.HasValue && joint is HingeJoint hinge && hinge.useLimits)
                {
                    var limits = hinge.limits;
                    limits.min = _ogLimits.Value.x;
                    limits.max = _ogLimits.Value.y;
                    hinge.limits = limits;
                }
            }
        }

        public static MoveThisControlInternal Create(MoveThisControl comp, ControlSpec control)
        {
            var real = comp.gameObject.AddComponent<MoveThisControlInternal>();
            real._control = control;
            real._localOffset = comp.LocalOffset;
            real._replaceCollider = comp.ReplacementStaticCollider;

            real._axis = comp.ChangeAxis ? comp.Axis : (Vector3?)null;
            real._limits = comp.ChangeLimits ? new Vector2(comp.Min, comp.Max) : (Vector2?)null;

            return real;
        }
    }
}
