using CarChanger.Common;
using CarChanger.Common.Components;
using UnityEditor;
using UnityEngine;

namespace CarChanger.Unity.Inspector
{
    [CustomEditor(typeof(UseDefaultMaterial))]
    internal class UseDefaultMaterialEditor : Editor
    {
        private UseDefaultMaterial _comp = null!;
        private SerializedProperty _mat = null!;
        private SerializedProperty _path = null!;
        private SerializedProperty _interior = null!;
        private SerializedProperty _procedural = null!;

        private GUIContent _matContent = new GUIContent("Material Name",
            "One of the default materials available ingame");
        private int _matPopup = 0;
        private GUIContent _proceduralContent = new GUIContent("Generate Exploded Material Procedurally",
            "Turns the material into a procedurally generated exploded version");

        private void OnEnable()
        {
            _mat = serializedObject.FindProperty(nameof(UseDefaultMaterial.Material));
            _path = serializedObject.FindProperty(nameof(UseDefaultMaterial.MaterialObjectPath));
            _interior = serializedObject.FindProperty(nameof(UseDefaultMaterial.FromInterior));
            _procedural = serializedObject.FindProperty(nameof(UseDefaultMaterial.GenerateExplodedMaterialProcedurally));
        }

        public override void OnInspectorGUI()
        {
            _comp = (UseDefaultMaterial)target;

            EditorGUILayout.PropertyField(_mat);
            var selected = (SourceMaterial)_mat.intValue;

            switch (selected)
            {
                case SourceMaterial.FromPath:
                    EditorGUILayout.PropertyField(_path);
                    EditorGUILayout.PropertyField(_interior);
                    break;
                case SourceMaterial.FromName:
                    _path.stringValue = EditorGUILayout.TextField(_matContent, _path.stringValue);

                    EditorGUILayout.BeginHorizontal();

                    _matPopup = EditorGUILayout.Popup(_matPopup, Constants.MaterialNames);

                    if (GUILayout.Button("Set", GUILayout.ExpandWidth(false)))
                    {
                        _path.stringValue = Constants.MaterialNames[_matPopup];
                    }

                    EditorGUILayout.EndHorizontal();
                    break;
                default:
                    break;
            }

            _procedural.boolValue = EditorGUILayout.Toggle(_proceduralContent, _procedural.boolValue);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
