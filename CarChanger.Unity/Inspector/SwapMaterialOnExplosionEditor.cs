using CarChanger.Common;
using CarChanger.Common.Components;
using UnityEditor;
using UnityEngine;

namespace CarChanger.Unity.Inspector
{
    [CustomEditor(typeof(SwapMaterialOnExplosion))]
    internal class SwapMaterialOnExplosionEditor : Editor
    {
        private SwapMaterialOnExplosion _comp = null!;
        private SerializedProperty _mat = null!;
        private SerializedProperty _defMat = null!;
        private SerializedProperty _path = null!;
        private SerializedProperty _interior = null!;
        private SerializedProperty _procedural = null!;
        private SerializedProperty _affected = null!;

        private GUIContent _matContent = new GUIContent("Material Name",
            "One of the default materials available ingame");
        private int _matPopup = 0;
        private GUIContent _proceduralContent = new GUIContent("Generate Exploded Material Procedurally",
            "Turns the material into a procedurally generated exploded version");

        private void OnEnable()
        {
            _mat = serializedObject.FindProperty(nameof(SwapMaterialOnExplosion.Material));
            _defMat = serializedObject.FindProperty(nameof(SwapMaterialOnExplosion.DefaultMaterial));
            _path = serializedObject.FindProperty(nameof(SwapMaterialOnExplosion.MaterialObjectPath));
            _interior = serializedObject.FindProperty(nameof(SwapMaterialOnExplosion.FromInterior));
            _procedural = serializedObject.FindProperty(nameof(SwapMaterialOnExplosion.GenerateExplodedMaterialProcedurally));
            _affected = serializedObject.FindProperty(nameof(SwapMaterialOnExplosion.AffectedGameObjects));
        }

        public override void OnInspectorGUI()
        {
            _comp = (SwapMaterialOnExplosion)target;

            EditorGUILayout.PropertyField(_mat);

            if (_mat.objectReferenceValue != null) goto END;

            EditorGUILayout.PropertyField(_defMat);
            var selected = (SourceMaterial)_defMat.intValue;

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

        END:

            _procedural.boolValue = EditorGUILayout.Toggle(_proceduralContent, _procedural.boolValue);
            EditorGUILayout.PropertyField(_affected);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
