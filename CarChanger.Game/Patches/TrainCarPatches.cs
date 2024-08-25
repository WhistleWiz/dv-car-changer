﻿using DV;
using HarmonyLib;

namespace CarChanger.Game.Patches
{
    [HarmonyPatch(typeof(TrainCar))]
    internal static class TrainCarPatches
    {
        [HarmonyPatch("Start"), HarmonyPrefix]
        private static void StartPrefix(TrainCar __instance)
        {
            if (__instance.TryGetComponent<AppliedChange>(out _))
            {
                return;
            }

            if (ChangeManager.DefaultConfigSettings.TryGetLivery(__instance.carLivery.id, out var liveryConfig) &&
                ChangeManager.LoadedConfigs.TryGetValue(__instance.carLivery, out var configs))
            {
                foreach (var item in liveryConfig.DefaultIds)
                {
                    if (!string.IsNullOrEmpty(item) &&
                        configs.TryFind(x => x.ModificationId == item, out var matching) &&
                        AppliedChange.CanApplyChange(__instance, matching))
                    {
                        __instance.gameObject.AddComponent<AppliedChange>().Config = matching;
                    }
                }

                return;
            }

            if (ChangeManager.LoadedConfigs.TryGetValue(__instance.carLivery, out configs))
            {
                // Component only added inside the if to not have an empty one.
                __instance.gameObject.AddComponent<AppliedChange>().Config = configs.GetRandomElement();
                return;
            }
        }
    }
}
