using System;
using UnityEngine;

namespace CarChanger.Common.Components
{
    public class WindowSetup : MonoBehaviour
    {
        public Vector2 Size = Vector2.one;
        public MeshRenderer[] Renderers = new MeshRenderer[0];
        public Action? DestroyEvent;

        private void OnDestroy()
        {
            DestroyEvent?.Invoke();
        }

        private void OnValidate()
        {
            transform.localScale = Vector3.one;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(Size.x, Size.y, 0.1f));
        }
    }
}
