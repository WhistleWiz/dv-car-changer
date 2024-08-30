using CarChanger.Common.Components;
using UnityEngine;

namespace CarChanger.Game
{
    internal class ComponentProcessor
    {
        public static void ProcessComponents(GameObject gameObject, MaterialHolder holder)
        {
            foreach (var item in gameObject.GetComponentsInChildren<UseBodyMaterial>())
            {
                ProcessBodyMaterial(item, holder);
            }
        }

        public static void ProcessBodyMaterial(UseBodyMaterial comp, MaterialHolder holder)
        {
            comp.GetRenderer().material = holder.GetMaterial(comp.Material, comp.MaterialObjectPath);
        }
    }
}
