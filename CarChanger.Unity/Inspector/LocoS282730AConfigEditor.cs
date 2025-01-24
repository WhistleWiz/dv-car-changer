using CarChanger.Common.Configs;
using UnityEditor;

namespace CarChanger.Unity.Inspector
{
    [CustomEditor(typeof(LocoS282730AConfig))]
    internal class LocoS282730AConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorHelper.DrawHeader("Additional Info");
            EditorGUILayout.Vector3Field("S282 Model Offset", LocoS282730AConfig.ModelOffset);
        }
    }
}
