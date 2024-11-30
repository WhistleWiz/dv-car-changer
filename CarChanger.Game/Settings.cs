using DV.Customization;
using DV.ThingTypes;
using LocoSim.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;

namespace CarChanger.Game
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        private const float IndentWidth = 30.0f;

        // Base game tenders to power.
        private static List<string> s_tenders = new List<string>()
        {
            "LocoS282B"
        };

        [Draw("Allow Tenders To Power Gadgets", Tooltip = "If true, the S282's tender will power gadgets (like lights)")]
        public bool TendersPowered = true;
        [Draw("Spawn With No Modification Chance", Tooltip = "The chance for a car to spawn without any modification\n" +
            "Does not apply to cars with a default modification set", Min = 0.0, Max = 1.0)]
        public float NoModificationChance = 0.0f;
        public DefaultConfigSettings DefaultConfigSettings = new DefaultConfigSettings();

        private static int s_selectedLivery = 0;
        private static Dictionary<DefaultConfigSettings.LiveryConfig, int> s_selectedConfigs = new Dictionary<DefaultConfigSettings.LiveryConfig, int>();

        internal void Initialise()
        {
            PowerBaseTenders();
        }

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);

            PowerBaseTenders();
        }

        public void OnChange() { }

        public void DrawGUI(UnityModManager.ModEntry modEntry)
        {
            this.Draw(modEntry);
            DrawConfigs();
        }

        private void DrawConfigs()
        {
            // Begin by setting a vertical group, that way everything inside lines up.
            GUILayout.BeginVertical(GUILayout.MinWidth(400), GUILayout.ExpandWidth(false));
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Presets",
                "Allows changing which configs will spawn by default for each car livery"), UnityModManager.UI.h2);
            
            // Help button linking to the wiki.
            GUILayout.Space(4);
            if (GUILayout.Button("Wiki", GUILayout.Width(IndentWidth * 2)))
            {
                Application.OpenURL("https://github.com/WhistleWiz/dv-car-changer/wiki/Preset-Modifications");
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(4);

            for (int i = 0; i < DefaultConfigSettings.Configs.Count; i++)
            {
                var config = DefaultConfigSettings.Configs[i];

                // Ensure the selected dictionary is updated.
                if (!s_selectedConfigs.ContainsKey(config))
                {
                    s_selectedConfigs.Add(config, 0);
                }

                // Need to make an horizontal group for every line to have labels...
                GUILayout.BeginHorizontal();

                // Button to remove the current entry.
                if (GUILayout.Button(new GUIContent("×", "Remove this livery"), GUILayout.Width(IndentWidth)))
                {
                    DefaultConfigSettings.Configs.RemoveAt(i);
                    s_selectedConfigs.Remove(config);
                    i--;
                    continue;
                }

                // Actual entry with label and text field. Highlight if livery does not exist.
                GUILayout.Label("Livery", GUILayout.ExpandWidth(false));
                bool hasLivery = DV.Globals.G.Types.TryGetLivery(config.LiveryName, out var livery);
                using (new GUIColorScope(newContent: hasLivery ? (Color?)null : Color.yellow))
                {
                    config.LiveryName = GUILayout.TextField(config.LiveryName);
                }

                GUILayout.EndHorizontal();

                // Allow Randoms option.
                GUILayout.BeginHorizontal();
                GUILayout.Space(IndentWidth * 2);

                GUILayout.Label(new GUIContent("Allow Randoms Together",
                    "Allows spawning of random modifications on top of the default settings"), GUILayout.ExpandWidth(false));
                config.AllowOthersOnTop = GUILayout.Toggle(config.AllowOthersOnTop, "", GUILayout.ExpandWidth(false));

                GUILayout.EndHorizontal();

                DrawConfigEntries(config, hasLivery, livery);
                DrawConfigButtons(config, hasLivery, livery, i);

                GUILayout.Space(10);
            }

            GUILayout.BeginHorizontal();

            var liveries = DV.Globals.G.Types.Liveries.Select(x => x.id).ToArray();

            // Show a popup to select a livery to add.
            UnityModManager.UI.PopupToggleGroup(ref s_selectedLivery, liveries, "All installed liveries");

            // Button to add new liveries.
            if (GUILayout.Button("Add New Livery ID"))
            {
                var config = new DefaultConfigSettings.LiveryConfig() { LiveryName = liveries[s_selectedLivery] };
                DefaultConfigSettings.Configs.Add(config);
                s_selectedConfigs.Add(config, 0);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private static void DrawConfigEntries(DefaultConfigSettings.LiveryConfig config, bool hasLivery, TrainCarLivery livery)
        {
            // Repeat the process used for each entry in this array.
            for (int i = 0; i < config.DefaultIds.Count; i++)
            {
                // Space for indentation.
                GUILayout.BeginHorizontal();
                GUILayout.Space(IndentWidth * 2);

                // Highlight if modification ID doesn't exist.
                GUILayout.Label("Modification", GUILayout.ExpandWidth(false));
                bool hasConfig = hasLivery && ChangeManager.TryGetConfig(livery, config.DefaultIds[i], out _);
                using (new GUIColorScope(newContent: hasConfig ? GUI.contentColor : Color.yellow))
                {
                    config.DefaultIds[i] = GUILayout.TextField(config.DefaultIds[i]);
                }

                GUILayout.EndHorizontal();
            }
        }

        private static void DrawConfigButtons(DefaultConfigSettings.LiveryConfig config, bool hasLivery, TrainCarLivery livery, int index)
        {
            string[] configs;
            int selected = s_selectedConfigs[config];

            if (hasLivery && ChangeManager.LoadedConfigs.TryGetValue(livery, out var loaded))
            {
                configs = loaded.Select(x => x.ModificationId).ToArray();
            }
            else
            {
                configs = new string[0];
            }

            // Add 2 buttons, one to add items to the array, another to remove them.
            GUILayout.BeginHorizontal();
            GUILayout.Space(IndentWidth * 2);

            // If there are configs for the current livery, show a selector popup.
            if (configs.Length > 0)
            {
                UnityModManager.UI.PopupToggleGroup(ref selected, configs, $"Installed changes for {config.LiveryName}", index);
            }

            // Update the currently selected one.
            s_selectedConfigs[config] = selected;

            // Add the selected config if it exists or an empty slot if it doesn't.
            if (GUILayout.Button(new GUIContent("+", "Add new modification"), GUILayout.Width(IndentWidth)))
            {
                if (configs.Length > 0)
                {
                    config.DefaultIds.Add(configs[selected]);
                }
                else
                {
                    config.DefaultIds.Add(string.Empty);
                }
            }

            // Disable remove button if the array is already empty.
            GUI.enabled = config.DefaultIds.Count > 0;

            if (GUILayout.Button(new GUIContent("-", "Remove last modification"), GUILayout.Width(IndentWidth)))
            {
                config.DefaultIds.RemoveAt(config.DefaultIds.Count - 1);
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();
        }

        private void PowerBaseTenders()
        {
            if (TendersPowered)
            {
                foreach (var item in s_tenders)
                {
                    if (DV.Globals.G.Types.TryGetLivery(item, out var livery))
                    {
                        if (!livery.prefab.TryGetComponent<TrainCarCustomization>(out var comp))
                        {
                            comp = livery.prefab.AddComponent<TrainCarCustomization>();
                        }

                        comp.electronicsFuseID = $"fuseboxDummy.ELECTRONICS_MAIN";
                    }
                }
            }
            else
            {
                foreach (var item in s_tenders)
                {
                    if (DV.Globals.G.Types.TryGetLivery(item, out var livery))
                    {
                        Helpers.DestroyIfNotNull(livery.prefab.GetComponent<TrainCarCustomization>());
                    }
                }
            }
        }
    }
}
