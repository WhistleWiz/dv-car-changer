﻿using CarChanger.Common.Components;
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
        }

        public static void ProcessBodyMaterial(UseBodyMaterial comp, MaterialHolder holder)
        {
            comp.GetRenderer().material = holder.GetMaterial(comp.Material, comp.MaterialObjectPath);
        }

        public static void ProcessHideTransforms(HideTransformsOnChange comp, Transform root)
        {
            comp.Hide(root);
        }

        public static void ProcessTelePassThroughColliders(GameObject gameObject)
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
