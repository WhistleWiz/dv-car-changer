using DV;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace CarChanger.Game.Patches
{
    [HarmonyPatch(typeof(CommsRadioController))]
    internal static class CommsRadioControllerPatches
    {
        [HarmonyPatch("Awake"), HarmonyPostfix]
        private static void AwakePostfix(CommsRadioController __instance)
        {
            var go = new GameObject("CommsRadioCarChanger");
            go.transform.parent = __instance.transform;
            go.SetActive(false);
            var mode = go.AddComponent<CommsRadioCarChanger>();
            mode.Controller = __instance;

            var t = typeof(CommsRadioController);
            var f = t.GetField("allModes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var value = (List<ICommsRadioMode>)f.GetValue(__instance);
            value.Add(mode);

            go.SetActive(true);
            __instance.ReactivateModes();
        }
    }
}
