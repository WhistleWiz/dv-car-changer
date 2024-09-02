using CarChanger.Common.Configs;
using UnityEditor;
using UnityEngine;

namespace CarChanger.Unity.Inspector
{
    [CustomEditor(typeof(LocoDE6Config))]
    internal class LocoDE6ConfigEditor : Editor
    {
        private LocoDE6Config _config = null!;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _config = (LocoDE6Config)target;

            EditorHelper.DrawHeader("Actions");

            if (GUILayout.Button("Reset Bogies"))
            {
                _config.ResetBogies();
                AssetHelper.SaveAsset(target);
            }

            if (GUILayout.Button("Reset Front Headlights"))
            {
                _config.ResetFrontHeadlights();
                AssetHelper.SaveAsset(target);
            }

            if (GUILayout.Button("Reset Rear Headlights"))
            {
                _config.ResetRearHeadlights();
                AssetHelper.SaveAsset(target);
            }
        }
    }
}
