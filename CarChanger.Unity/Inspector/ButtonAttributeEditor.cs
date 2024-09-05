using CarChanger.Common;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CarChanger.Unity.Inspector
{
    [CustomPropertyDrawer(typeof(ButtonAttribute), true)]
    internal class ButtonAttributeEditor : PropertyDrawer
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private MethodInfo? _method;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var att = (ButtonAttribute)attribute;
            label.text = att.Text ?? label.text;

            position = new Rect(position.x + (position.width - att.Width) * 0.5f, position.y, att.Width, position.height);

            if (GUI.Button(position, label))
            {
                if (_method == null)
                {
                    var t = property.serializedObject.targetObject.GetType();
                    _method = t.GetMethod(att.Target, Flags);

                    if (_method == null)
                    {
                        Debug.LogError($"Could not find method {att.Target} in {t.Name}");
                        base.OnGUI(position, property, label);
                        return;
                    }
                }

                _method.Invoke(property.serializedObject.targetObject, null);
                AssetHelper.SaveAsset(property.serializedObject.targetObject);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
