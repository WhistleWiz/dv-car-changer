using DV;
using HarmonyLib;
using System.Collections.Generic;

namespace CarChanger.Game.Patches
{
    [HarmonyPatch(typeof(CommsRadioController))]
    internal static class CommsRadioControllerPatches
    {
        [HarmonyPatch("Awake"), HarmonyPostfix]
        private static void AwakePostfix(CommsRadioController __instance)
        {
            // Create the object as inactive to prevent Awake() from running too early.
            var go = Helpers.CreateEmptyInactiveObject("CommsRadioCarChanger", __instance.transform, false);
            var mode = go.AddComponent<CommsRadioCarChanger>();
            mode.Controller = __instance;

            // Force the new mode into the private list of modes...
            var t = typeof(CommsRadioController);
            var f = t.GetField("allModes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ((List<ICommsRadioMode>)f.GetValue(__instance)).Add(mode);

            // Reactivate the GO with the new mode and refresh the controller.
            go.SetActive(true);
            __instance.ReactivateModes();
        }
    }
}
