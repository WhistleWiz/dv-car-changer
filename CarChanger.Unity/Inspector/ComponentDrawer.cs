using CarChanger.Common.Components;
using UnityEditor;
using UnityEngine;

namespace CarChanger.Unity.Inspector
{
    internal class ComponentDrawer
    {
        private static Color Transparent = new Color(1.0f, 1.0f, 1.0f, 0.15f);

        [DrawGizmo(GizmoType.Selected)]
        private static void DrawGizmoRandomisePosition(RandomisePosition comp, GizmoType gizmoType)
        {
            Handles.color = new Color(0.8f, 0.9f, 1.0f);
            Handles.matrix = Matrix4x4.TRS(comp.transform.position, comp.transform.rotation, Vector3.one);
            Handles.DrawWireCube(Vector3.zero, comp.PositionRange);

            Handles.color = Color.red;
            Handles.DrawWireArc(Vector3.zero, Vector3.right, Vector3.forward, comp.RotationRange.x, 0.5f);
            Handles.DrawWireArc(Vector3.zero, Vector3.right, Vector3.forward, -comp.RotationRange.x, 0.5f);
            Handles.color *= Transparent;
            Handles.DrawSolidArc(Vector3.zero, Vector3.right, Vector3.forward, comp.RotationRange.x, 0.5f);
            Handles.DrawSolidArc(Vector3.zero, Vector3.right, Vector3.forward, -comp.RotationRange.x, 0.5f);
            Handles.color = Color.green;
            Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, comp.RotationRange.y, 0.5f);
            Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, -comp.RotationRange.y, 0.5f);
            Handles.color *= Transparent;
            Handles.DrawSolidArc(Vector3.zero, Vector3.up, Vector3.forward, comp.RotationRange.y, 0.5f);
            Handles.DrawSolidArc(Vector3.zero, Vector3.up, Vector3.forward, -comp.RotationRange.y, 0.5f);
            Handles.color = Color.blue;
            Handles.DrawWireArc(Vector3.zero, Vector3.forward, Vector3.up, comp.RotationRange.z, 0.5f);
            Handles.DrawWireArc(Vector3.zero, Vector3.forward, Vector3.up, -comp.RotationRange.z, 0.5f);
            Handles.color *= Transparent;
            Handles.DrawSolidArc(Vector3.zero, Vector3.forward, Vector3.up, comp.RotationRange.z, 0.5f);
            Handles.DrawSolidArc(Vector3.zero, Vector3.forward, Vector3.up, -comp.RotationRange.z, 0.5f);
        }
    }
}
