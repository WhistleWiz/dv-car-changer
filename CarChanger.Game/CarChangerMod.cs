using DVLangHelper.Runtime;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace CarChanger.Game
{
    public static class CarChangerMod
    {
        public const string Guid = "wiz.carchanger";

        public static UnityModManager.ModEntry Instance { get; private set; } = null!;
        public static TranslationInjector Translations { get; private set; } = null!;
        public static Dictionary<string, AudioClip> SoundCache { get; private set; } = null!;
        public static Dictionary<string, Material> MaterialCache { get; private set; } = null!;
        public static Settings Settings { get; private set; } = null!;

        // Unity Mod Manager Wiki: https://wiki.nexusmods.com/index.php/Category:Unity_Mod_Manager
        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            Instance = modEntry;
            Translations = new TranslationInjector(Guid);
            Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);

            Instance.OnGUI += Settings.DrawGUI;
            Instance.OnSaveGUI += Settings.Save;

            BuildCache();

            Localization.Inject();

            ScanMods();
            UnityModManager.toggleModsListen += HandleModToggled;

            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            return true;
        }

        private static void ScanMods()
        {
            foreach (var mod in UnityModManager.modEntries)
            {
                if (mod.Active)
                {
                    ChangeManager.LoadChanges(mod);
                }
            }
        }

        private static void HandleModToggled(UnityModManager.ModEntry modEntry, bool newState)
        {
            if (newState)
            {
                ChangeManager.LoadChanges(modEntry);
            }
        }

        public static void Log(string message)
        {
            Instance.Logger.Log(message);
        }

        public static void Warning(string message)
        {
            Instance.Logger.Warning(message);
        }

        public static void Error(string message)
        {
            Instance.Logger.Error(message);
        }

        private static void BuildCache()
        {
            // For timing.
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            // For sounds just take the first one, they are unique.
            SoundCache = Resources.FindObjectsOfTypeAll<AudioClip>()
                    .GroupBy(x => x.name, StringComparer.Ordinal)
                    .ToDictionary(k => k.Key, v => v.First());

            sw.Stop();
            Log($"Built sound cache {sw.Elapsed.TotalSeconds:F4}");
            sw.Restart();

            MaterialCache = new Dictionary<string, Material>();

            // For materials, first group by material name.
            var mats = Resources.FindObjectsOfTypeAll<Material>()
                    .GroupBy(x => x.name, StringComparer.Ordinal);

            foreach (var group in mats)
            {
                // If there's only 1 material per name, add it directly.
                if (group.Count() == 1)
                {
                    MaterialCache.Add(group.Key, group.First());
                    continue;
                }

                // For multiple materials with the same name (ex. Glass), group by the texture name too.
                var organised = group.GroupBy(x => x.mainTexture.name, StringComparer.Ordinal);

                // Finally add the first element of these new groups. If it's still the same name just
                // ignore that material, too much hassle.
                foreach (var newGroup in organised)
                {
                    MaterialCache.Add($"{group.Key}/{newGroup.Key}", newGroup.First());
                }
            }

            sw.Stop();
            Log($"Built material cache cache {sw.Elapsed.TotalSeconds:F4}");

            //Log($"\"{string.Join("\",\n\"", MaterialCache.Keys)}\"");
        }
    }
}
