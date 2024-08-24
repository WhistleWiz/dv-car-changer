using UnityEngine;

namespace CarChanger.Common
{
    /// <summary>
    /// Add this to a bogie prefab to include the <see cref="Renderer"/> in the same <see cref="GameObject"/> in the glowy bits.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class ExtraBrakeRenderer : MonoBehaviour
    {
        public Renderer GetRenderer() => GetComponent<Renderer>();
    }
}
