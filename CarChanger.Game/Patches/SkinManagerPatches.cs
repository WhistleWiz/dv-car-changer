using CarChanger.Common;
using DV.ThingTypes;
using HarmonyLib;
using System.Collections.Generic;
using UnityModManagerNet;

namespace CarChanger.Game.Patches
{
    [HarmonyPatch]
    internal static class SkinManagerPatches
    {
        //private static Dictionary<ModelConfig, Dictionary<string, string>> s_cache = new Dictionary<ModelConfig, Dictionary<string, string>>();

        //[HarmonyPrepare, HarmonyPatch("SkinManagerMod.SkinProvider", "GetCarTextureDictionary")]
        //private static bool PrepareSkinManager()
        //{
        //    return UnityModManager.FindMod(ModIntegrations.SkinManager.Id) != null;
        //}

        //[HarmonyPostfix, HarmonyPatch("SkinManagerMod.SkinProvider", "GetCarTextureDictionary")]
        //private static void GetCarTextureDictionaryPostfix(ref Dictionary<string, string> __result, TrainCarLivery carType)
        //{
        //    if (!ChangeManager.LoadedConfigs.TryGetValue(carType, out var configs)) return;

        //    CarChangerMod.Log($"Patching GetCarTextureDictionary for livery '{carType.id}'");

        //    foreach (var item in configs)
        //    {
        //        if (!s_cache.TryGetValue(item, out var textures))
        //        {
        //            textures = ModIntegrations.SkinManager.GetRendererTextureNames(Helpers.AllMeshRenderers(item));
        //            s_cache.Add(item, textures);
        //        }

        //        foreach (var tex in textures)
        //        {
        //            if (__result.ContainsKey(tex.Key)) continue;

        //            __result.Add(tex.Key, tex.Value);
        //        }
        //    }

        //    CarChangerMod.Log($"Textures: {string.Join(", ", __result.Keys)}");
        //}
    }
}
