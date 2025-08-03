using UnityEngine;

namespace CarChanger.Common.Components
{
    [AddComponentMenu("Car Changer/Paint Material")]
    public class PaintMaterial : MonoBehaviour
    {
        public enum TargetArea : byte
        {
            Exterior,
            Interior
        }

        public Material Material = null!;
        public TargetArea Area = TargetArea.Exterior;
        public string[] Paints = new string[0];
        public Renderer[] AffectedRenderers = new Renderer[0];
        [Button(nameof(SetupRenderers), "Auto Setup Renderers"), SerializeField]
        private bool _setRenderers;

        private void SetupRenderers()
        {
            AffectedRenderers = GetComponentsInChildren<Renderer>();
        }
    }
}
