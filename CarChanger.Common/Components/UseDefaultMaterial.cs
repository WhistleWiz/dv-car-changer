using UnityEngine;
using static CarChanger.Common.Components.PaintMaterial;

namespace CarChanger.Common.Components
{
    [AddComponentMenu("Car Changer/Use Default Material")]
    [RequireComponent(typeof(Renderer))]
    public class UseDefaultMaterial : MonoBehaviour
    {
        public SourceMaterial Material;
        [Tooltip("Path to an object with the material")]
        public string MaterialObjectPath = string.Empty;
        [Tooltip("If the path is in the interior object (for interior and interactables)")]
        public bool FromInterior;
        [EnableIf(nameof(EnableProcedural))]
        public bool GenerateExplodedMaterialProcedurally = false;
        [Tooltip("If true, this material is only used when the current paint is one of the ones below")]
        public bool UseForPaint = false;
        [EnableIf(nameof(UseForPaint))]
        public TargetArea Area = TargetArea.Exterior;
        [EnableIf(nameof(UseForPaint))]
        public string[] Paints = new string[0];

        public Renderer GetRenderer() => gameObject.GetComponent<Renderer>();

        private bool EnableProcedural() => Material switch
        {
            SourceMaterial.BrokenWindows => false,
            _ => Material >= SourceMaterial.BodyExploded,
        };
    }
}
