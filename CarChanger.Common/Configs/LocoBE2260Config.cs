using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/BE2-260 (Microshunter) Modification", order = Constants.MenuOrderConstants.Electric + 0)]
    public class LocoBE2260Config : CarWithInteriorAndBogiesConfig
    {
        private static readonly Vector3 OriginalFrontBeamPosition = new Vector3(0, 0, 0);
        private static readonly Vector3 OriginalRearBeamPosition = new Vector3(0, 0, 0);

        protected override float OriginalRadius => Constants.Wheels.RadiusDE2480;

        [Header("Doors and Windows")]
        public GameObject? Door;
        public GameObject? DoorExploded;
        public bool HideOriginalCabDoor = false;
        public GameObject? WindowFront;
        public GameObject? WindowFrontExploded;
        public GameObject? WindowRear;
        public GameObject? WindowRearExploded;
        public bool HideOriginalWindows = false;

        [Header("Headlights")]
        public bool UseCustomFrontHeadlights = false;
        [EnableIf(nameof(UseCustomFrontHeadlights))]
        public Mesh? FrontMesh;
        [EnableIf(nameof(UseCustomFrontHeadlights))]
        public Vector3 FrontBeamPosition = OriginalFrontBeamPosition;
        [Button(nameof(ResetFrontHeadlights), "Reset"), SerializeField]
        private bool _resetFrontButton;

        public bool UseCustomRearHeadlights = false;
        [EnableIf(nameof(UseCustomRearHeadlights))]
        public Mesh? RearMesh;
        [EnableIf(nameof(UseCustomRearHeadlights))]
        public Vector3 RearBeamPosition = OriginalRearBeamPosition;
        [Button(nameof(ResetRearHeadlights), "Reset"), SerializeField]
        private bool _resetRearButton;

        private void ResetFrontHeadlights()
        {
            FrontMesh = null;
            FrontBeamPosition = OriginalFrontBeamPosition;
        }

        private void ResetRearHeadlights()
        {
            RearMesh = null;
            RearBeamPosition = OriginalRearBeamPosition;
        }

        public override bool DoValidation(out string error)
        {
            if (FrontBogie || RearBogie)
            {
                var result = Validation.ValidateBothBogies(FrontBogie, RearBogie, 1, 1, 1, 1);

                if (!string.IsNullOrEmpty(result))
                {
                    error = result!;
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        public static bool CanCombine(LocoBE2260Config a, LocoBE2260Config b) =>
            CarWithInteriorAndBogiesConfig.CanCombine(a, b) &&
            !(a.HideOriginalCabDoor && b.HideOriginalCabDoor) &&
            !(a.HideOriginalWindows && b.HideOriginalWindows) &&
            !(a.UseCustomFrontHeadlights && b.UseCustomFrontHeadlights) &&
            !(a.UseCustomRearHeadlights && b.UseCustomRearHeadlights);
    }
}
