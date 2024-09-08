using CarChanger.Common.Components;
using CarChanger.Game.Components;
using DV.CabControls.Spec;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game
{
    internal class ComponentProcessor
    {
        public static void ProcessComponents(GameObject gameObject, MaterialHolder holder)
        {
            if (gameObject == null) return;

            // Find the root car or interior transform.
            Transform? root = null;
            var interior = gameObject.GetComponentInParent<TrainCarInteriorObject>();

            if (interior != null)
            {
                root = interior.transform;
            }
            else
            {
                var car = TrainCar.Resolve(gameObject);

                if (car != null)
                {
                    root = car.transform;
                }
            }

            foreach (var item in gameObject.GetComponentsInChildren<UseBodyMaterial>())
            {
                ProcessBodyMaterial(item, holder);
            }

            if (root != null)
            {
                foreach (var item in gameObject.GetComponentsInChildren<HideTransformsOnChange>())
                {
                    ProcessHideTransforms(item, root);
                }
            }

            if (gameObject.TryGetComponent(out MoveThisControl moveThis))
            {
                ProcessMoveThisControl(moveThis);
            }
        }

        public static void ProcessBodyMaterial(UseBodyMaterial comp, MaterialHolder holder)
        {
            comp.GetRenderer().material = holder.GetMaterial(comp.Material, comp.MaterialObjectPath);
        }

        public static void ProcessHideTransforms(HideTransformsOnChange comp, Transform root)
        {
            comp.Hide(root);
        }

        public static void ProcessMoveThisControl(MoveThisControl comp)
        {
            var control = comp.GetComponentInParent<ControlSpec>();

            if (control != null)
            {
                MoveThisControlInternal.Create(comp, control);
            }
            else
            {
                CarChangerMod.Error($"No control found to move on {comp.name}");
            }

            Object.Destroy(comp);
        }

        public static void ProcessTeleportPassThroughColliders(GameObject gameObject)
        {
            foreach (var item in gameObject.GetComponentsInChildren<TeleportPassthroughCollider>())
            {
                var comp = item.gameObject.AddComponent<TeleportArcPassThrough>();
                comp.twoSided = item.TwoSided;
                comp.colliders = item.OtherColliders.ToHashSet();
            }
        }
    }
}
