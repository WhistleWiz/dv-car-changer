using CarChanger.Common;
using CarChanger.Game.Components;
using DV;
using DV.CabControls.Spec;
using DV.Customization;
using HarmonyLib;
using LocoSim.Definitions;
using System.Collections;
using System.Collections.Generic;
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
            //if (CarChangerMod.Settings.TendersPowered &&
            //    car.carLivery.parentType.kind.id == "Tender" &&
            //    !car.TryGetComponent(out TrainCarCustomization comp)
            //    && car.gameObject.TryGetComponentInChildren<IndependentFusesDefinition>(out var fuses))
            //{
            //    comp = car.gameObject.AddComponent<TrainCarCustomization>();
            //    comp.electronicsFuseID = $"{fuses.ID}.{fuses.fuses[0].id}";
            //}

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
            var liveryConfigs = CarChangerMod.Settings.DefaultConfigSettings.Configs.FindAll(x => x.LiveryName == car.carLivery.id);

            if (liveryConfigs.Count > 0 && ChangeManager.LoadedConfigs.TryGetValue(car.carLivery, out var configs))
            {
                // Pick a random config setup.
                var liveryConfig = liveryConfigs.GetRandomElement();
                var added = new List<ModelConfig>();

                foreach (var item in liveryConfig.DefaultIds)
                {
                    if (!string.IsNullOrEmpty(item) &&
                        configs.TryFind(x => x.ModificationId == item, out var matching) &&
                        AppliedChange.CanApplyChange(car, matching))
                    {
                        AppliedChange.AddChange(car, matching);
                        added.Add(matching);
                    }
                }

                // If randoms are not allowed, stop here.
                if (!liveryConfig.AllowOthersOnTop) return;

                // Only allow configs that aren't in use already.
                var others = configs.FindAll(config => AppliedChange.CanApplyChange(car, config) && !added.Contains(config));

                if (others.Count > 0)
                {
                    AppliedChange.AddChange(car, others.GetRandomElement());
                }

                return;
            }

            // Finally, as last resort, load a random change.
            if (!Helpers.Chance(CarChangerMod.Settings.NoModificationChance) &&
                ChangeManager.LoadedConfigs.TryGetValue(car.carLivery, out configs))
            {
                AppliedChange.AddChange(car, configs.GetRandomElement());
                return;
            }
        }

        private static void DespawnSequence(TrainCar car)
        {
            foreach (var change in car.GetAppliedChanges())
            {
                Object.Destroy(change);
            }
        }
    }
}
