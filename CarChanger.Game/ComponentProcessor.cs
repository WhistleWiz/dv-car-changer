using CarChanger.Common.Components;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game
{
    internal class ComponentProcessor
    {
        public static void ProcessComponents(GameObject gameObject, MaterialHolder holder)
        {
            if (gameObject == null) return;

            foreach (var item in gameObject.GetComponentsInChildren<UseBodyMaterial>())
            {
                ProcessBodyMaterial(item, holder);
            }
        }

        public static void ProcessBodyMaterial(UseBodyMaterial comp, MaterialHolder holder)
        {
            comp.GetRenderer().material = holder.GetMaterial(comp.Material, comp.MaterialObjectPath);
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
