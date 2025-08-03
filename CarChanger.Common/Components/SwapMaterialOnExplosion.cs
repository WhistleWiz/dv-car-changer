using UnityEngine;

namespace CarChanger.Common.Components
{
    [AddComponentMenu("Car Changer/Swap Material On Explosion")]
    public class SwapMaterialOnExplosion : MonoBehaviour
    {
        public Material Material = null!;
        [Tooltip("Use a default material instead of a custom one")]
        public SourceMaterial DefaultMaterial;
        [Tooltip("Path to an object with the material")]
        public string MaterialObjectPath = string.Empty;
        [Tooltip("If the path is in the interior object (for interior and interactables)")]
        public bool FromInterior;
        [EnableIf(nameof(EnableProcedural)), Tooltip("Turns the material into a procedurally generated exploded version")]
        public bool GenerateExplodedMaterialProcedurally = false;

        public GameObject[] AffectedGameObjects = new GameObject[0];

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
