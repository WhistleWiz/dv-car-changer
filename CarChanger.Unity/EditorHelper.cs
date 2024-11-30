using CarChanger.Common;
using UnityEditor;
using UnityEngine;

namespace CarChanger.Unity
{
    internal static class EditorHelper
    {
        private static int s_popupIndex = 0;

        public static void DrawHeader(string title)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        public static void DrawHeader(string title, string tooltip)
        {
            DrawHeader(new GUIContent(title, tooltip));
        }

        public static void DrawHeader(GUIContent content)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(content, EditorStyles.boldLabel);
        }

        public static string TextFieldWithPopup(GUIContent content, string text, string[] options)
        {
            text = EditorGUILayout.TextField(content, text);

            EditorGUILayout.BeginHorizontal();

            if (s_popupIndex >= options.Length)
            {
                s_popupIndex = 0;
            }

            s_popupIndex = EditorGUILayout.Popup(s_popupIndex, options);

            if (GUILayout.Button("Set", GUILayout.ExpandWidth(false)))
            {
                text = options[s_popupIndex];
            }

            EditorGUILayout.EndHorizontal();

            return text;
        }
    }
}
