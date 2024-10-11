using System;
using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/S282-730A Modification", order = Constants.MenuOrderConstants.Steam + 1)]
    public class LocoS282730AConfig : CarWithInteriorConfig
    {
        [Flags]
        public enum ChangeRegions
        {
            None                = 0,
            Pilot               = 1 << 0,
            SmokeDeflectors     = 1 << 1,
            SmokeboxDoor        = 1 << 2,
            FrontSteps          = 1 << 3,
            FrontHandrail       = 1 << 4,
            SideHandrails       = 1 << 5,
            Chimney             = 1 << 6,
            Cylinders           = 1 << 7,
            SandDome            = 1 << 8,
            SteamDome           = 1 << 9,
            AirTanks            = 1 << 10,
            Turret              = 1 << 11,
            Firebox             = 1 << 12,
            RunningBoard        = 1 << 13,
            
            Cab                 = 1 << 27,
            CabRoof             = 1 << 28,
            Whistles            = 1 << 29,
            Bell                = 1 << 30,
            Buffers             = 1 << 31,
        }

        private static readonly Vector3 OriginalBeamPosition = new Vector3(-0.00146591f, 3.579122f, 10.98136f);

        [Header("Doors and Windows")]
        public GameObject? LeftWindow;
        public GameObject? RightWindow;
        public GameObject? LeftWindowExploded;
        public GameObject? RightWindowExploded;
        public bool HideOriginalWindows = false;
        public GameObject? ToolboxLid;
        public GameObject? ToolboxLidExploded;
        public bool HideOriginalToolboxLid = false;

        [Header("Headlights")]
        public bool UseCustomHeadlights = false;
        [EnableIf(nameof(UseCustomHeadlights))]
        public Mesh Mesh = null!;
        [EnableIf(nameof(UseCustomHeadlights))]
        public Vector3 BeamPosition = OriginalBeamPosition;
        [Button(nameof(ResetHeadlights), "Reset Position"), SerializeField]
        private bool _resetHeadlight;

        [Header("Change Regions"), Tooltip("Which areas does this change block\n" +
            "Other changes with the same areas selected are marked as incompatible with this one")]
        public ChangeRegions BlockedRegions = ChangeRegions.None;

        public void ResetHeadlights() => BeamPosition = OriginalBeamPosition;

        public override bool DoValidation(out string error)
        {
            error = string.Empty;
            return true;
        }

        public static bool CanCombine(LocoS282730AConfig a, LocoS282730AConfig b) =>
            CarWithInteriorConfig.CanCombine(a, b) &&
            !(a.HideOriginalWindows && b.HideOriginalWindows) &&
            !(a.HideOriginalToolboxLid && !b.HideOriginalToolboxLid) &&
            !(a.UseCustomHeadlights && b.UseCustomHeadlights) &&
            (a.BlockedRegions & b.BlockedRegions) == 0;
    }
}
