using UnityEngine;

namespace CarChanger.Common
{
    public class ButtonAttribute : PropertyAttribute
    {
        public string Target { get; }
        public string? Text { get; set; }
        public float Width { get; set; } = 200.0f;

        /// <summary>
        /// Enables or disables the property in the inspector based on a condition.
        /// </summary>
        /// <param name="target">The name of the method that decides when to enable/disable. Must return a bool.</param>
        public ButtonAttribute(string target, string? text = null)
        {
            Target = target;
            Text = text;
        }
    }
}
