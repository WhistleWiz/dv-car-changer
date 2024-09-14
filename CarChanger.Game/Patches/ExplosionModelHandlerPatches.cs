using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CarChanger.Game.Patches
{
    [HarmonyPatch(typeof(ExplosionModelHandler))]
    internal static class ExplosionModelHandlerPatches
    {
        private static FieldInfo? s_swapsField;
        private static FieldInfo SwapsField = Helpers.GetCached(ref s_swapsField, () =>
            typeof(ExplosionModelHandler).GetField("materialSwaps", BindingFlags.Instance | BindingFlags.NonPublic));

        [HarmonyPatch(nameof(ExplosionModelHandler.HandleExplosionModelChange)), HarmonyPrefix]
        private static void HandleExplosionModelChangePrefix(ExplosionModelHandler __instance)
        {
            var matSwaps = (ExplosionModelHandler.MaterialSwapData[])SwapsField.GetValue(__instance);

            foreach (var swap in matSwaps)
            {
                // No need to change anything. The issue is when this is null in the game.
                if (swap.affectedRenderers != null) continue;

                swap.affectedRenderers = new List<(Renderer, Material)>();

                foreach (GameObject go in swap.affectedGameObjects)
                {
                    // The only difference to what the game does is including inactive, which happens from the changes.
                    foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>(true))
                    {
                        swap.affectedRenderers.Add((renderer, renderer.sharedMaterial));
                        renderer.sharedMaterial = swap.swapMaterial;
                    }
                }
            }
        }

        [HarmonyPatch(nameof(ExplosionModelHandler.RevertToUnexplodedModel)), HarmonyPostfix]
        private static void RevertToUnexplodedModelPostfix(ExplosionModelHandler __instance)
        {
            // Only apply this to the real one.
            if (!__instance.TryGetComponent(out TrainCar car)) return;

            foreach (var item in car.GetAppliedChanges())
            {
                item.ForceApplyChange("unexplode");
            }
        }
    }
}
