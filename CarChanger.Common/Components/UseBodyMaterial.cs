using UnityEngine;

namespace CarChanger.Common.Components
{
    [RequireComponent(typeof(Renderer))]
    public class UseBodyMaterial : MonoBehaviour
    {
        public SourceMaterial Material;
        [EnableIf(nameof(EnablePath)), Tooltip("Path to an object with the material\n" +
            "Use the selector for default vehicles")]
        public string MaterialObjectPath = string.Empty;

        private bool EnablePath() => Material == SourceMaterial.Custom;
    }
}
