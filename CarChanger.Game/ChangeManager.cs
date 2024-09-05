using CarChanger.Common;
using CarChanger.Common.Configs;
using DV.ThingTypes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityModManagerNet;

namespace CarChanger.Game
{
    public static class ChangeManager
    {
        internal static Dictionary<TrainCarLivery, List<ModelConfig>> LoadedConfigs = new Dictionary<TrainCarLivery, List<ModelConfig>>();
        internal static DefaultConfigSettings DefaultConfigSettings = new DefaultConfigSettings();
        internal static Dictionary<string, List<string>> SaveData = new Dictionary<string, List<string>>();

        private static TrainCarType_v2? s_lastLoadedType;
        private static TrainCarLivery? s_lastLoadedLivery;

        internal static void LoadConfigFile()
        {
            if (File.Exists(Path.Combine(CarChangerMod.Instance.Path, Constants.ConfigFile)))
            {
                using var reader = File.OpenText(Path.Combine(CarChangerMod.Instance.Path, Constants.ConfigFile));
                var result = JsonConvert.DeserializeObject<DefaultConfigSettings>(reader.ReadToEnd());

                if (result != null)
                {
                    DefaultConfigSettings = result;
                }
                else
                {
                    CarChangerMod.Error("Failure loading default configs file.");
                    DefaultConfigSettings = new DefaultConfigSettings();
                }
            }
            else
            {
                CarChangerMod.Log("Default configs file not found, creating new one...");
                WriteSettingsFile();
            }
        }

        internal static void WriteSettingsFile()
        {
            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };

            using (var file = File.Create(Path.Combine(CarChangerMod.Instance.Path, Constants.ConfigFile)))
            using (var streamWriter = new StreamWriter(file))
            using (var jsonWr = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(jsonWr, DefaultConfigSettings);
            }
        }

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
                        if (pax.Livery == PassengerType.All)
                        {
                            AddToCarType(DV.Globals.G.Types.TrainCarType_to_v2[(TrainCarType)PassengerType.Blue].parentType, item);
                            continue;
                        }

                        AddToCarLivery(DV.Globals.G.Types.TrainCarType_to_v2[(TrainCarType)PassengerType.Blue], item);
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
                    case LocoDE6Config _:
                        AddToCarType(DV.Globals.G.Types.TrainCarType_to_v2[TrainCarType.LocoDiesel].parentType, item);
                        continue;
                    case LocoS282AConfig _:
                        AddToCarType(DV.Globals.G.Types.TrainCarType_to_v2[TrainCarType.LocoSteamHeavy].parentType, item);
                        continue;
                    case CustomCarConfig ccl:
                        if (!string.IsNullOrEmpty(ccl.CarTypeId) && DV.Globals.G.Types.TryGetCarType(ccl.CarTypeId, out var type))
                        {
                            AddToCarType(type, item);
                            continue;
                        }
                        if (DV.Globals.G.Types.TryGetLivery(ccl.LiveryId, out var livery))
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
