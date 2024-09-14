﻿using UnityEngine;

namespace CarChanger.Common.Components
{
    [RequireComponent(typeof(Renderer))]
    public class UseBodyMaterial : MonoBehaviour
    {
        public SourceMaterial Material;
        [EnableIf(nameof(EnablePath)), Tooltip("Path to an object with the material")]
        public string MaterialObjectPath = string.Empty;
        [EnableIf(nameof(EnablePath)), Tooltip("If the path is in the interior object (for interior and interactables)")]
        public bool FromInterior;
        [EnableIf(nameof(EnableProcedural)), Tooltip("Turns the material into a procedurally generated exploded version")]
        public bool GenerateExplodedMaterialProcedurally = false;

        public Renderer GetRenderer() => gameObject.GetComponent<Renderer>();

        private bool EnablePath() => Material == SourceMaterial.FromPath;

        private bool EnableProcedural()
        {
            switch (Material)
            {
                case SourceMaterial.BodyExploded:
                case SourceMaterial.InteriorExploded:
                case SourceMaterial.InteriorExtraExploded:
                case SourceMaterial.FromPath:
                    return true;
                default:
                    return false;
            }
        }
    }
}
