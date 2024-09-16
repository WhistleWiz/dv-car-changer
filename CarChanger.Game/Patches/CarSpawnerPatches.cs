using CarChanger.Game.Components;
using DV;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace CarChanger.Game.Patches
{
    [HarmonyPatch(typeof(CarSpawner))]
    internal static class CarSpawnerPatches
    {
        [HarmonyPatch("Awake"), HarmonyPostfix]
        private static void AwakePostfix(CarSpawner __instance)
        {
            __instance.CarSpawned += SpawnSequence;
            __instance.CarAboutToBeDeleted += DespawnSequence;
        }

        private static void SpawnSequence(TrainCar car)
        {
            // If the car already has changes applied here, skip changing them.
            if (car.TryGetComponent<AppliedChange>(out _))
            {
                return;
            }

            car.StartCoroutine(WaitForGUIDCoroutine(car));
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
                        AppliedChange.AddChange(car, config);
                    }
                    else
                    {
                        CarChangerMod.Warning($"Could not load saved config '{item}' for '{car.carLivery.id}'");
                    }
                }

                return;
            }

            // If the car is not in a save (it's new), try to see if there's a default setting for it from the config file.
            if (CarChangerMod.Settings.DefaultConfigSettings.TryGetLivery(car.carLivery.id, out var liveryConfig) &&
                ChangeManager.LoadedConfigs.TryGetValue(car.carLivery, out var configs))
            {
                foreach (var item in liveryConfig.DefaultIds)
                {
                    if (!string.IsNullOrEmpty(item) &&
                        configs.TryFind(x => x.ModificationId == item, out var matching) &&
                        AppliedChange.CanApplyChange(car, matching))
                    {
                        AppliedChange.AddChange(car, matching);
                    }
                }

                return;
            }

            // Finally, as last resort, load a random change.
            if (!Helpers.Chance(CarChangerMod.Settings.NoModificationChance) &&
                ChangeManager.LoadedConfigs.TryGetValue(car.carLivery, out configs))
            {
                AppliedChange.AddChange(car,  configs.GetRandomElement());
                return;
            }
        }

        private static void DespawnSequence(TrainCar car)
        {
            var changes = car.GetAppliedChanges();

            foreach (var change in changes)
            {
                Object.Destroy(change);
            }
        }
    }
}
