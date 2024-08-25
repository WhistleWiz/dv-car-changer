using CarChanger.Common;
using DVLangHelper.Runtime;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
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

        // Unity Mod Manager Wiki: https://wiki.nexusmods.com/index.php/Category:Unity_Mod_Manager
        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            Instance = modEntry;
            Translations = new TranslationInjector(Guid);
            BuildCache();
            ChangeManager.LoadConfigFile();

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
            SoundCache = Resources.FindObjectsOfTypeAll<AudioClip>()
                    .GroupBy(x => x.name, StringComparer.Ordinal)
                    .ToDictionary(k => k.Key, v => v.First());
        }
    }
}
