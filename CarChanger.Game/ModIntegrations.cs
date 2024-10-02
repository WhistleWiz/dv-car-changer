using System;
using System.IO;
using System.Reflection;

namespace CarChanger.Game
{
    internal static class ModIntegrations
    {
        public const BindingFlags FlagsStaticPrivate = BindingFlags.Static | BindingFlags.NonPublic;

        public static class ZCouplers
        {
            private static Type s_typeSettings;
            private static Type s_typeKnuckles;
            private static MethodInfo? s_method;
            private static object s_settings;
            private static bool s_loaded = false;

            public static bool Loaded => s_loaded;

            static ZCouplers()
            {
                string path = $"{Directory.GetCurrentDirectory()}\\Mods\\ZCouplers\\ZCouplers.dll";

                if (!File.Exists(path))
                {
                    CarChangerMod.Log($"ZCouplers is not installed, skipping integration.");
                    s_typeSettings = null!;
                    s_typeKnuckles = null!;
                    s_settings = null!;
                    return;
                }

                try
                {
                    Assembly assembly = Assembly.LoadFile(path);
                    s_typeSettings = assembly.GetType("DvMod.ZCouplers.Settings");
                    s_typeKnuckles = assembly.GetType("DvMod.ZCouplers.KnuckleCouplers");
                    s_method = s_typeKnuckles.GetMethod("ToggleBuffers", FlagsStaticPrivate);

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

                if (s_method == null)
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
                if (!Loaded)
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


                s_method?.Invoke(null, new object[] { car.gameObject, car.carLivery, visible });
            }
        }
    }
}
