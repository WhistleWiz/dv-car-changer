using CarChanger.Common;
using CarChanger.Common.Configs;
using DV.ThingTypes;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityModManagerNet;

namespace CarChanger.Game
{
    public static class ChangeManager
    {
        internal static Dictionary<TrainCarLivery, List<ModelConfig>> LoadedConfigs = new Dictionary<TrainCarLivery, List<ModelConfig>>();
        internal static Dictionary<string, List<string>> SaveData = new Dictionary<string, List<string>>();

        private static TrainCarType_v2? s_lastLoadedType;
        private static TrainCarLivery? s_lastLoadedLivery;

        internal static void LoadChanges(UnityModManager.ModEntry mod)
        {
            AssetBundle bundle;
            var files = Directory.EnumerateFiles(mod.Path, Constants.Bundle, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                bundle = AssetBundle.LoadFromFile(file);

                // Somehow failed to load the bundle.
                if (bundle == null)
                {
                    CarChangerMod.Error("Failed to load bundle!");
                    continue;
                }

                foreach (var item in bundle.LoadAllAssets<CarChangePack>())
                {
                    LoadConfigs(item.PackConfigs);
                }

                bundle.Unload(false);
            }
        }

        private static void LoadConfigs(ModelConfig[] configs)
        {
            foreach (var item in configs)
            {
                item.AfterLoad();

                // Find the correct type to apply the change to.
                switch (item)
                {
                    case WagonConfig wagon:
                        if (wagon.CarType == WagonType.UseLivery)
                        {
                            AddToCarLivery(DV.Globals.G.Types.TrainCarType_to_v2[(TrainCarType)wagon.CarLivery], item);
                            continue;
                        }

                        AddToCarType(Helpers.EnumToCarType(wagon.CarType), item);
                        continue;
                    case PassengerConfig pax:
                        switch (pax.Livery)
                        {
                            case PassengerType.All:
                                AddToCarType(DV.Globals.G.Types.TrainCarType_to_v2[(TrainCarType)PassengerType.Blue].parentType, item);
                                continue;
                            case PassengerType.Red:
                            case PassengerType.Green:
                            case PassengerType.Blue:
                                AddToCarLivery(DV.Globals.G.Types.TrainCarType_to_v2[(TrainCarType)pax.Livery], item);
                                continue;
                            case PassengerType.FirstClass:
                                AddToCarLivery(DV.Globals.G.Types.TrainCarType_to_v2[(TrainCarType)PassengerType.Green], item);
                                continue;
                            case PassengerType.SecondClass:
                                AddToCarLivery(DV.Globals.G.Types.TrainCarType_to_v2[(TrainCarType)PassengerType.Red], item);
                                AddToCarLivery(DV.Globals.G.Types.TrainCarType_to_v2[(TrainCarType)PassengerType.Blue], item);
                                continue;
                            default:
                                CarChangerMod.Error($"Unknown passenger livery {pax.Livery}");
                                continue;
                        }
                    case CabooseConfig _:
                        AddToCarType(DV.Globals.G.Types.TrainCarType_to_v2[TrainCarType.CabooseRed].parentType, item);
                        continue;

                    case LocoDE2480Config _:
                        AddToCarType(DV.Globals.G.Types.TrainCarType_to_v2[TrainCarType.LocoShunter].parentType, item);
                        continue;
                    case LocoDE6860Config _:
                        AddToCarType(DV.Globals.G.Types.TrainCarType_to_v2[TrainCarType.LocoDiesel].parentType, item);
                        continue;
                    case LocoDH4670Config _:
                        AddToCarType(DV.Globals.G.Types.TrainCarType_to_v2[TrainCarType.LocoDH4].parentType, item);
                        continue;
                    case LocoDM3540Config _:
                        AddToCarType(DV.Globals.G.Types.TrainCarType_to_v2[TrainCarType.LocoDM3].parentType, item);
                        continue;

                    case LocoS060440Config _:
                        AddToCarType(DV.Globals.G.Types.TrainCarType_to_v2[TrainCarType.LocoS060].parentType, item);
                        continue;
                    case LocoS282730AConfig _:
                        AddToCarType(DV.Globals.G.Types.TrainCarType_to_v2[TrainCarType.LocoSteamHeavy].parentType, item);
                        continue;
                    case LocoS282730BConfig _:
                        AddToCarType(DV.Globals.G.Types.TrainCarType_to_v2[TrainCarType.Tender].parentType, item);
                        continue;

                    case LocoBE2260Config _:
                        AddToCarType(DV.Globals.G.Types.TrainCarType_to_v2[TrainCarType.LocoMicroshunter].parentType, item);
                        continue;

                    case LocoDE6860SlugConfig _:
                        AddToCarType(DV.Globals.G.Types.TrainCarType_to_v2[TrainCarType.LocoDE6Slug].parentType, item);
                        continue;
                    case LocoHandcarConfig _:
                        AddToCarType(DV.Globals.G.Types.TrainCarType_to_v2[TrainCarType.LocoShunter].parentType, item);
                        continue;

                    case ModificationGroupConfig group:
                        LoadConfigs(group.ModificationsToActivate);

                        if (s_lastLoadedType != null)
                        {
                            AddToCarType(s_lastLoadedType, item);
                        }
                        else if (s_lastLoadedLivery != null)
                        {
                            AddToCarLivery(s_lastLoadedLivery, item);
                        }
                        break;
                    case CustomCarConfig ccc:
                        if (!string.IsNullOrEmpty(ccc.CarTypeId) && DV.Globals.G.Types.TryGetCarType(ccc.CarTypeId, out var type))
                        {
                            AddToCarType(type, item);
                            continue;
                        }
                        if (DV.Globals.G.Types.TryGetLivery(ccc.LiveryId, out var livery))
                        {
                            AddToCarLivery(livery, item);
                        }
                        continue;
                    default:
                        continue;
                }
            }
        }

        private static void AddToCarType(TrainCarType_v2 type, ModelConfig config)
        {
            foreach (var livery in type.liveries)
            {
                AddToCarLivery(livery, config);
            }

            s_lastLoadedLivery = null;
            s_lastLoadedType = type;
        }

        private static void AddToCarLivery(TrainCarLivery livery, ModelConfig config)
        {
            if (LoadedConfigs.TryGetValue(livery, out var values))
            {
                if (!values.Contains(config))
                {
                    values.Add(config);
                }
            }
            else
            {
                values = new List<ModelConfig> { config };
                LoadedConfigs.Add(livery, values);
            }

            CarChangerMod.Translations.AddTranslations(config.LocalizationKey, config.ModificationName);
            CarChangerMod.Log($"Loaded config {config.ModificationId} for {livery.id}");

            s_lastLoadedLivery = livery;
            s_lastLoadedType = null;
        }

        /// <summary>
        /// Tries to find a modification with the specified ID for a livery.
        /// </summary>
        /// <param name="livery">The <see cref="TrainCarLivery"/> with the modification.</param>
        /// <param name="id">The ID of the modification.</param>
        /// <param name="config">The modification if found, otherwise <see cref="null"/>.</param>
        /// <returns>True if the livery has a modification with the supplied ID, false otherwise.</returns>
        public static bool TryGetConfig(TrainCarLivery livery, string id, out ModelConfig config)
        {
            if (LoadedConfigs.TryGetValue(livery, out var configs))
            {
                return configs.TryFind(x => x.ModificationId == id, out config);
            }

            config = null!;
            return false;
        }
    }
}
