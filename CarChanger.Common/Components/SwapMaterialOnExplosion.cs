using UnityEngine;

namespace CarChanger.Common.Components
{
    public class SwapMaterialOnExplosion : MonoBehaviour
    {
        public Material Material = null!;
        [EnableIf(nameof(EnableMat)), Tooltip("Use a default material instead of a custom one")]
        public SourceMaterial DefaultMaterial;
        [EnableIf(nameof(EnablePath)), Tooltip("Path to an object with the material")]
        public string MaterialObjectPath = string.Empty;
        [EnableIf(nameof(EnableProcedural)), Tooltip("Turns the material into a procedurally generated exploded version")]
        public bool GenerateExplodedMaterialProcedurally = false;

        public GameObject[] AffectedGameObjects = new GameObject[0];

        private bool EnableMat() => Material == null;
        private bool EnablePath() => EnableMat() && DefaultMaterial == SourceMaterial.FromPath;

        private bool EnableProcedural()
        {
            if (Material != null) return true;

            return DefaultMaterial switch
            {
                SourceMaterial.BrokenWindows => false,
                _ => DefaultMaterial >= SourceMaterial.BodyExploded,
            };
        }
    }
}
