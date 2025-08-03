using System;
using UnityEngine;

namespace CarChanger.Common.Components
{
    [AddComponentMenu("Car Changer/Customization Placement Meshes")]
    public class CustomizationPlacementMeshes : MonoBehaviour
    {
        public MeshFilter[] CollisionMeshes = new MeshFilter[0];
        public MeshFilter[] DrillDisableMeshes = new MeshFilter[0];

        public Action? ClearListOnDestroy = null;

        private void OnDestroy()
        {
            ClearListOnDestroy?.Invoke();
        }
    }
}
