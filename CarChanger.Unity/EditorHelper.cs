using UnityEditor;
using UnityEngine;

namespace CarChanger.Unity
{
    internal static class EditorHelper
    {
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
    }
}
