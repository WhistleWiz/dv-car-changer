﻿using System;
using UnityEngine;
using UnityModManagerNet;

namespace CarChanger.Game
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        private const float IndentWidth = 24.0f;

        [Draw("Spawn With No Modification Chance", Tooltip = "The chance for a car to spawn without any modification\n" +
            "Does not apply to cars with a default modification set", Min = 0.0, Max = 1.0)]
        public float NoModificationChance = 0.0f;
        public DefaultConfigSettings DefaultConfigSettings = new DefaultConfigSettings();

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
            var richStyle = GUI.skin.label;
            richStyle.richText = true;

            // Begin by setting a vertical group, that way everything inside lines up.
            GUILayout.BeginVertical(GUILayout.MinWidth(400), GUILayout.ExpandWidth(false));
            GUILayout.Label(new GUIContent("<color=lightblue><b>Default Configs</b></color>",
                "Allows changing which configs will spawn by default for each car livery"), richStyle);
            GUILayout.Space(4);

            for (int i = 0; i < DefaultConfigSettings.Configs.Count; i++)
            {
                var config = DefaultConfigSettings.Configs[i];

                // Need to make an horizontal group for every line to have labels...
                GUILayout.BeginHorizontal();

                // Button to remove the current entry.
                if (GUILayout.Button(new GUIContent("-", "Remove this livery"), GUILayout.Width(IndentWidth)))
                {
                    DefaultConfigSettings.Configs.RemoveAt(i);
                    i--;
                    continue;
                }

                // Actual entry with label and text field.
                GUILayout.Label("Livery", GUILayout.ExpandWidth(false));
                config.LiveryName = GUILayout.TextField(config.LiveryName);

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(IndentWidth * 2);

                GUILayout.Label(new GUIContent("Allow Randoms Together",
                    "Allows spawning of random modifications on top of the default settings"), GUILayout.ExpandWidth(false));
                config.AllowOthersOnTop = GUILayout.Toggle(config.AllowOthersOnTop, "", GUILayout.ExpandWidth(false));

                GUILayout.EndHorizontal();

                // Repeat the process used for each entry in this array.
                for (int j = 0; j < config.DefaultIds.Count; j++)
                {
                    // Space for indentation.
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(IndentWidth * 2);

                    GUILayout.Label("Modification", GUILayout.ExpandWidth(false));
                    config.DefaultIds[j] = GUILayout.TextField(config.DefaultIds[j]);

                    GUILayout.EndHorizontal();
                }

                // Add 2 buttons, one to add items to the array, another to remove them.
                GUILayout.BeginHorizontal();
                GUILayout.Space(IndentWidth * 2);

                if (GUILayout.Button("+", GUILayout.Width(IndentWidth)))
                {
                    config.DefaultIds.Add(string.Empty);
                }

                // Disable remove button if the array is already empty.
                GUI.enabled = config.DefaultIds.Count > 0;

                if (GUILayout.Button("-", GUILayout.Width(IndentWidth)))
                {
                    config.DefaultIds.RemoveAt(config.DefaultIds.Count - 1);
                }

                GUI.enabled = true;

                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }

            // Button to add new liveries.
            if (GUILayout.Button("Add New Livery ID"))
            {
                DefaultConfigSettings.Configs.Add(new DefaultConfigSettings.LiveryConfig());
            }

            GUILayout.EndVertical();
        }
    }
}
