using DV.ThingTypes;
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

        [Draw("Spawn With No Modification Chance", Tooltip = "The chance for a car to spawn without any modification\n" +
            "Does not apply to cars with a default modification set", Min = 0.0, Max = 1.0)]
        public float NoModificationChance = 0.0f;
        public DefaultConfigSettings DefaultConfigSettings = new DefaultConfigSettings();

        [NonSerialized]
        private int _selectedLivery = 0;
        [NonSerialized]
        private Dictionary<string, int> _selectedConfigs = new Dictionary<string, int>();

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
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
            GUILayout.Label(new GUIContent("Preset Modifications",
                "Allows changing which configs will spawn by default for each car livery"), UnityModManager.UI.h2);
            GUILayout.Space(4);

            for (int i = 0; i < DefaultConfigSettings.Configs.Count; i++)
            {
                var config = DefaultConfigSettings.Configs[i];

                // Need to make an horizontal group for every line to have labels...
                GUILayout.BeginHorizontal();

                // Button to remove the current entry.
                if (GUILayout.Button(new GUIContent("×", "Remove this livery"), GUILayout.Width(IndentWidth)))
                {
                    DefaultConfigSettings.Configs.RemoveAt(i);
                    i--;
                    continue;
                }

                // Actual entry with label and text field. Highlight if livery does not exist.
                GUILayout.Label("Livery", GUILayout.ExpandWidth(false));
                bool hasLivery = DV.Globals.G.Types.TryGetLivery(config.LiveryName, out var livery);
                using (new GUIColorScope(newContent: hasLivery ? GUI.contentColor : Color.yellow))
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
                DrawConfigButtons(config, hasLivery, livery, _selectedConfigs);

                GUILayout.Space(10);
            }

            GUILayout.BeginHorizontal();

            var liveries = DV.Globals.G.Types.Liveries.Select(x => x.id).ToArray();

            // Show a popup to select a livery to add.
            UnityModManager.UI.PopupToggleGroup(ref _selectedLivery, liveries);

            // Button to add new liveries.
            if (GUILayout.Button("Add New Livery ID"))
            {
                DefaultConfigSettings.Configs.Add(new DefaultConfigSettings.LiveryConfig() { LiveryName = liveries[_selectedLivery] });
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

        private static void DrawConfigButtons(DefaultConfigSettings.LiveryConfig config, bool hasLivery, TrainCarLivery livery, Dictionary<string, int> selectedConfigs)
        {
            string[] configs;
            int selected = 0;

            if (hasLivery && ChangeManager.LoadedConfigs.TryGetValue(livery, out var loaded))
            {
                configs = loaded.Select(x => x.ModificationId).ToArray();

                // For actual liveries add or get the entry for the selected value.
                if (!selectedConfigs.TryGetValue(livery.id, out selected))
                {
                    selected = 0;
                    selectedConfigs.Add(livery.id, 0);
                }
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
                UnityModManager.UI.PopupToggleGroup(ref selected, configs);

                // Update the currently selected one.
                if (selectedConfigs.ContainsKey(livery.id))
                {
                    selectedConfigs[livery.id] = selected;
                }
            }

            if (GUILayout.Button("+", GUILayout.Width(IndentWidth)))
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

            if (GUILayout.Button("-", GUILayout.Width(IndentWidth)))
            {
                config.DefaultIds.RemoveAt(config.DefaultIds.Count - 1);
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();
        }
    }
}
