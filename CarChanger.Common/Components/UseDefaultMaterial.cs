using UnityEngine;

namespace CarChanger.Common.Components
{
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

        public Renderer GetRenderer() => gameObject.GetComponent<Renderer>();

        private bool EnableProcedural() => Material switch
        {
            SourceMaterial.BrokenWindows => false,
            _ => Material >= SourceMaterial.BodyExploded,
        };
    }
}
