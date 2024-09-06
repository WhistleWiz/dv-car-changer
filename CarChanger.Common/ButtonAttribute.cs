using UnityEngine;

namespace CarChanger.Common
{
    public class ButtonAttribute : PropertyAttribute
    {
        /// <summary>
        /// The target method.
        /// </summary>
        public string Target { get; }
        /// <summary>
        /// The text on the button. If null, the name of the field will be used.
        /// </summary>
        public string? Text { get; set; }
        /// <summary>
        /// The width of the button. Set it to a negative value to stretch and fill all available width instead.
        /// </summary>
        public float Width { get; set; } = -1.0f;

        /// <summary>
        /// Adds a button below the field that can activate a method with no parameters.
        /// </summary>
        /// <param name="target">The name of the method to call when the button is clicked.</param>
        /// <param name="text">The text on the button. If null, the name of the field will be used.</param>
        public ButtonAttribute(string target, string? text = null)
        {
            Target = target;
            Text = text;
        }
    }
}
