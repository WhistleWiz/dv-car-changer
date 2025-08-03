using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using static UnityEngine.EventSystems.EventTrigger;

namespace CarChanger.Game
{
    internal static class ModIntegrations
    {
        public const BindingFlags FlagsStaticPrivate = BindingFlags.Static | BindingFlags.NonPublic;
        public const BindingFlags FlagsStaticPublic = BindingFlags.Static | BindingFlags.Public;

        public static bool IsModActive(string modId)
        {
            return UnityModManager.modEntries.TryFind(x => x.Info.Id == modId, out var mod) && mod.Active;
        }

        public static class ZCouplers
        {
            public const string Id = "ZCouplers";
            public const string AltId = "ZCouplers_Updated";

            private static UnityModManager.ModEntry s_entry;
            private static Type s_typeSettings;
            private static Type s_typeKnuckles;
            private static MethodInfo? s_toggleMMethod;
            private static object s_settings;
            private static bool s_loaded = false;

            public static bool Loaded => s_loaded;

            static ZCouplers()
            {
                if (!UnityModManager.modEntries.TryFind(x => x.Info.Id == Id || x.Info.Id == AltId, out s_entry))
                {
                    CarChangerMod.Log($"ZCouplers is not installed, skipping integration.");
                    s_typeSettings = null!;
                    s_typeKnuckles = null!;
                    s_settings = null!;
                    return;
                }

                try
                {
                    var assembly = s_entry.Assembly;
                    s_typeSettings = assembly.GetType("DvMod.ZCouplers.Settings");
                    s_typeKnuckles = assembly.GetType("DvMod.ZCouplers.KnuckleCouplers");
                    s_toggleMMethod = s_typeKnuckles.GetMethod("ToggleBuffers", FlagsStaticPrivate);

                    var tm = assembly.GetType("DvMod.ZCouplers.Main");
                    var ts = assembly.GetType("DvMod.ZCouplers.Settings");
                    s_settings = tm.GetField("settings").GetValue(null);
                }
                catch (Exception e)
                {
                    CarChangerMod.Warning($"Could not load ZCouplers ({e.Message})");
                    s_typeSettings = null!;
                    s_typeKnuckles = null!;
                    s_settings = null!;
                    return;
                }

                if (s_toggleMMethod == null)
                {
                    CarChangerMod.Warning($"Could not load ZCouplers (method is null)");
                    return;
                }

                if (s_settings == null)
                {
                    CarChangerMod.Warning($"Could not load ZCouplers (settings is null)");
                    // Yeah...
                    s_settings = null!;
                    return;
                }

                s_loaded = true;
            }

            public static void HandleBuffersToggled(TrainCar car)
            {
                if (!Loaded || !IsModActive(Id))
                {
                    return;
                }

                bool visible;

                try
                {
                    // Skip the rest if knuckles aren't enabled.
                    if (!(bool)s_typeKnuckles.GetField("enabled").GetValue(null)) return;

                    visible = (bool)s_typeSettings.GetField("showBuffersWithKnuckles").GetValue(s_settings);
                }
                catch (Exception e)
                {
                    CarChangerMod.Warning($"ZCouplers integration failed ({e.Message})");
                    s_loaded = false;
                    return;
                }

                s_toggleMMethod?.Invoke(null, new object[] { car.gameObject, car.carLivery, visible });
            }
        }

        public static class Gauge
        {
            public const string Id = "Gauge";

            private static UnityModManager.ModEntry s_entry;
            private static MethodInfo? s_method;
            private static bool s_loaded = false;

            public static bool Loaded => s_loaded;

            static Gauge()
            {
                if (!UnityModManager.modEntries.TryFind(x => x.Info.Id == Id, out s_entry))
                {
                    CarChangerMod.Log($"Gauge is not installed, skipping integration.");
                    return;
                }

                try
                {
                    var t = s_entry.Assembly.GetType("Gauge.Patches.Bogie_Start_Patch");
                    s_method = t.GetMethod("Postfix", FlagsStaticPrivate);
                }
                catch (Exception e)
                {
                    CarChangerMod.Warning($"Could not load Gauge ({e.Message})");
                    return;
                }

                s_loaded = true;
            }

            public static void RegaugeBogie(Bogie bogie)
            {
                if (!Loaded || !IsModActive(Id))
                {
                    return;
                }

                if (s_method == null)
                {
                    CarChangerMod.Error("Gauge call failed! Setting Gauge loaded to false.");
                    s_loaded = false;
                    return;
                }

                s_method?.Invoke(null, new[] { bogie });
            }
        }

        public static class SkinManager
        {
            public const string Id = "SkinManagerMod";

            private static MethodInfo? s_method;

            public static Dictionary<string, string> GetRendererTextureNames(IEnumerable<MeshRenderer> renderers)
            {
                if (s_method == null)
                {
                    string path = $"{Directory.GetCurrentDirectory()}\\Mods\\SkinManagerMod\\SkinManagerMod.dll";

                    Assembly assembly = Assembly.LoadFile(path);
                    var t = assembly.GetType("SkinManagerMod.TextureUtility");
                    s_method = t.GetMethod("GetRendererTextureNames", FlagsStaticPublic);
                }

                return (Dictionary<string, string>)s_method.Invoke(null, new[] { renderers });
            }
        }
    }
}
