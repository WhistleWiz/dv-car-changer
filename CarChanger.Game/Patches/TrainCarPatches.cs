using DV;
using HarmonyLib;
using System.Collections;

namespace CarChanger.Game.Patches
{
    [HarmonyPatch(typeof(TrainCar))]
    internal static class TrainCarPatches
    {
        [HarmonyPatch("Start"), HarmonyPostfix]
        private static void StartPostfix(TrainCar __instance)
        {
            // If the car already has changes applied here, skip changing them.
            if (__instance.TryGetComponent<AppliedChange>(out _))
            {
                return;
            }

            __instance.StartCoroutine(WaitForGUIDCoroutine(__instance));
        }

        private static IEnumerator WaitForGUIDCoroutine(TrainCar car)
        {
            while (car.logicCar == null)
            {
                yield return null;
            }

            DoInitialSetup(car);
        }

        private static void DoInitialSetup(TrainCar car)
        {
            // Check if this car had saved configs from a previous session.
            if (ChangeManager.SaveData.TryGetValue(car.CarGUID, out var savedConfigs))
            {
                foreach (var item in savedConfigs)
                {
                    if (ChangeManager.TryGetConfig(car.carLivery, item, out var config))
                    {
                        car.gameObject.AddComponent<AppliedChange>().Config = config;
                    }
                    else
                    {
                        CarChangerMod.Warning($"Could not load saved config '{item}' for '{car.carLivery.id}'");
                    }
                }

                return;
            }

            // If the car is not in a save (it's new), try to see if there's a default setting for it from the config file.
            if (ChangeManager.DefaultConfigSettings.TryGetLivery(car.carLivery.id, out var liveryConfig) &&
                ChangeManager.LoadedConfigs.TryGetValue(car.carLivery, out var configs))
            {
                foreach (var item in liveryConfig.DefaultIds)
                {
                    if (!string.IsNullOrEmpty(item) &&
                        configs.TryFind(x => x.ModificationId == item, out var matching) &&
                        AppliedChange.CanApplyChange(car, matching))
                    {
                        car.gameObject.AddComponent<AppliedChange>().Config = matching;
                    }
                }

                return;
            }

            // Finally, as last resort, load a random change.
            if (ChangeManager.LoadedConfigs.TryGetValue(car.carLivery, out configs))
            {
                // Component only added inside the if to not have an empty one.
                car.gameObject.AddComponent<AppliedChange>().Config = configs.GetRandomElement();
                return;
            }
        }
    }
}
