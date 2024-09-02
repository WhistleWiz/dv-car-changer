using CarChanger.Common;
using System.Reflection;
using System.Security.Policy;
using UnityEditor;
using UnityEngine;

namespace CarChanger.Unity.Inspector
{
    [CustomPropertyDrawer(typeof(EnableIfAttribute), true)]
    internal class EnableIfAttributeEditor : PropertyDrawer
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var att = (EnableIfAttribute)attribute;
            var t = property.serializedObject.targetObject.GetType();
            var m = t.GetMethod(att.Target, Flags);

            if (m == null)
            {
                Debug.LogError($"Could not find method {att.Target} in {t.Name}");
                base.OnGUI(position, property, label);
                return;
            }

            var result = (bool)m.Invoke(property.serializedObject.targetObject, null);

            EditorGUI.BeginProperty(position, label, property);
            GUI.enabled = att.Invert ? !result : result;
            EditorGUI.PropertyField(position, property, true);
            GUI.enabled = true;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
