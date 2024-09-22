using CarChanger.Common.Components;
using DV.CabControls.Spec;
using DV.Interaction;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game.Components
{
    internal class ChangeThisControlInternal : MonoBehaviour
    {
        // Prevent an infinite loop.
        private const int Safety = 1024;

        private ControlSpec _control = null!;
        private Vector3 _localOffset = Vector3.zero;
        private Vector2? _limits;
        private Vector3? _axis;
        private List<GameObject> _replaceColliders = new List<GameObject>();
        private List<GameObject> _disabledColliders = new List<GameObject>();
        private GameObject? _replaceStaticCollider;
        private List<GameObject> _disabledStaticColliders = new List<GameObject>();
        private Transform? _replaceInteraction;
        private Transform? _ogInteraction;
        private bool _changed = false;
        private Vector2? _ogLimits;
        private Vector3? _ogAxis;

        private IEnumerator Start()
        {
            Joint joint;
            int counter = Safety;

            // Wait for the joint to get created.
            while (!_control.TryGetComponent(out joint))
            {
                if (counter-- < 0)
                {
                    CarChangerMod.Warning($"Failed to get joint for ChangeThisControl '{name}'");
                    yield break;
                }

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

                if (_control.TryGetComponent(out SteppedJoint stepped))
                {
                    Helpers.ChangeSteppedJointLimits(hinge, stepped);
                }
            }

            // Change the control's colliders.
            if (_replaceColliders.Count > 0)
            {
                _disabledColliders = _control.colliderGameObjects.ToList();

                foreach (var item in _disabledColliders)
                {
                    item.gameObject.SetActive(false);
                }

                foreach (var item in _replaceColliders)
                {
                    item.SetLayersRecursive(_control.gameObject.layer);
                }

                _control.colliderGameObjects = _replaceColliders.ToArray();
                FixGrabColliders();
            }

            // Change the collider of the StaticInteractionArea if possible.
            if (_replaceStaticCollider != null)
            {
                var ia = Helpers.GetInteractionArea(_control);

                if (ia != null)
                {
                    foreach (var item in ia.GetComponentsInChildren<Collider>(true))
                    {
                        if (!item.gameObject.activeSelf) continue;

                        item.gameObject.SetActive(false);
                        _disabledStaticColliders.Add(item.gameObject);
                    }

                    _replaceStaticCollider.transform.SetParent(ia.transform, false);
                    _replaceStaticCollider.SetLayersRecursive(ia.gameObject.layer);
                }
                else
                {
                    // Clean out the colliders if they are not supported.
                    Destroy(_replaceStaticCollider);
                }
            }

            // Replace the interaction point.
            if (_replaceInteraction != null)
            {
                _ogInteraction = Helpers.ReplaceInteractionPoint(_control, _replaceInteraction);
            }

            _changed = true;
        }

        private void OnDestroy()
        {
            if (!_changed || _control == null || gameObject == null)
            {
                return;
            }

            Helpers.MoveControl(_control, -_localOffset);
            var joint = _control.GetComponent<Joint>();

            // Return the original colliders.
            foreach (var item in _disabledColliders)
            {
                item.gameObject.SetActive(true);
            }

            _control.colliderGameObjects = _disabledColliders.ToArray();
            FixGrabColliders();

            // Restore the static interaction area.
            Helpers.DestroyIfNotNull(_replaceStaticCollider);

            foreach (var item in _disabledStaticColliders)
            {
                item.gameObject.SetActive(true);
            }

            // Restore the joint.
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

                    if (_control.TryGetComponent(out SteppedJoint stepped))
                    {
                        Helpers.ChangeSteppedJointLimits(hinge, stepped);
                    }
                }
            }

            // Restore the interaction point.
            if (_ogInteraction != null)
            {
                Helpers.ReplaceInteractionPoint(_control, _ogInteraction);
            }
        }

        private void FixGrabColliders()
        {
            if (!_control.TryGetComponent(out AGrabHandler handler)) return;

            handler.interactionColliders.Clear();

            foreach (var go in _control.colliderGameObjects)
            {
                if (go == null) continue;

                foreach (Collider collider in go.GetComponents<Collider>())
                {
                    handler.interactionColliders.Add(collider);
                }
            }

            // Keep this warning.
            if (handler.interactionColliders.Count == 0)
            {
                Debug.LogError("SetInteractionColliders found no colliders in colliderGameObjects. This should not happen. Interaction will probably be possible.", this);
            }
        }

        public static ChangeThisControlInternal Create(ChangeThisControl comp, ControlSpec control)
        {
            var real = comp.gameObject.AddComponent<ChangeThisControlInternal>();
            real._control = control;
            real._localOffset = comp.LocalOffset;
            real._replaceColliders = comp.ReplacementColliders;
            real._replaceStaticCollider = comp.ReplacementStaticCollider;
            real._replaceInteraction = comp.ReplacementInteractionPoint;

            real._axis = comp.ChangeAxis ? comp.Axis : (Vector3?)null;
            real._limits = comp.ChangeLimits ? new Vector2(comp.Min, comp.Max) : (Vector2?)null;

            return real;
        }
    }
}
