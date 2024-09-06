using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/S282A Modification", order = Constants.MenuOrderConstants.Steam + 1)]
    public class LocoS282AConfig : CarWithInteriorConfig
    {
        private static readonly Vector3 OriginalBeamPosition = new Vector3(0.0f, 3.579122f, 10.98136f);

        [Header("Doors and Windows")]
        public GameObject? LeftWindow = null;
        public GameObject? RightWindow = null;
        public GameObject? LeftWindowExploded = null;
        public GameObject? RightWindowExploded = null;
        public bool HideOriginalWindows = false;
        public GameObject? ToolboxLid = null;
        public GameObject? ToolboxLidExploded = null;
        public bool HideOriginalToolboxLid = false;

        [Header("Headlights")]
        public bool UseCustomHeadlights = false;
        [EnableIf(nameof(EnableHeadlights))]
        public Mesh Mesh = null!;
        [EnableIf(nameof(EnableHeadlights))]
        public Vector3 BeamPosition = OriginalBeamPosition;
        [Button(nameof(ResetHeadlights), "Reset Position"), SerializeField]
        private bool _resetHeadlight;

        public override bool DoValidation(out string error)
        {
            error = string.Empty;
            return true;
        }

        public void ResetHeadlights()
        {
            BeamPosition = OriginalBeamPosition;
        }

        private bool EnableHeadlights() => UseCustomHeadlights;

        public static bool CanCombine(LocoS282AConfig a, LocoS282AConfig b)
        {
            return CarWithInteriorConfig.CanCombine(a, b);
        }
    }
}
