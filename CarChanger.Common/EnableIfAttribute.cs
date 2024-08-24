using UnityEngine;

namespace CarChanger.Common
{
    /// <summary>
    /// Enables or disables the property in the inspector based on a condition.
    /// </summary>
    public class EnableIfAttribute : PropertyAttribute
    {
        public string Target;
        public bool Invert;

        /// <summary>
        /// Enables or disables the property in the inspector based on a condition.
        /// </summary>
        /// <param name="target">The method that decides when to enable/disable.</param>
        /// <param name="invert">If true, the result of <paramref name="target"/> will have the opposite effect.</param>
        public EnableIfAttribute(string target, bool invert = false)
        {
            Target = target;
            Invert = invert;
        }
    }
}
