using UnityEngine;

namespace CarChanger.Common.Components
{
    [RequireComponent(typeof(Renderer))]
    public class UseBodyMaterial : MonoBehaviour
    {
        public SourceMaterial Material;
        [EnableIf(nameof(EnablePath)), Tooltip("Path to an object with the material")]
        public string MaterialObjectPath = string.Empty;

        public Renderer GetRenderer() => gameObject.GetComponent<Renderer>();

        private bool EnablePath() => Material == SourceMaterial.FromPath;
    }
}
