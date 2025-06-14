using System;
using System.Collections.Generic;
using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/S282-730A Modification", order = Constants.MenuOrderConstants.Steam + 1)]
    public class LocoS282730AConfig : CarWithInteriorConfig
    {
        public static readonly Vector3 ModelOffset = new Vector3(0, 0, 4.880929f);

        [Flags]
        public enum ChangeRegions
        {
            None                = 0,
            Cowcatcher          = 1 << 0,
            Pilot               = 1 << 1,
            SmokeDeflectors     = 1 << 2,
            SmokeboxDoor        = 1 << 3,
            FrontSteps          = 1 << 4,
            FrontHandrail       = 1 << 5,
            SideHandrails       = 1 << 6,
            Chimney             = 1 << 7,
            Cylinders           = 1 << 8,
            SandDome            = 1 << 9,
            SteamDome           = 1 << 10,
            AirTanks            = 1 << 11,
            Turret              = 1 << 12,
            Firebox             = 1 << 13,
            RunningBoard        = 1 << 14,
            
            Cab                 = 1 << 27,
            CabRoof             = 1 << 28,
            Whistles            = 1 << 29,
            Bell                = 1 << 30,
            Buffers             = 1 << 31,
        }

        private static readonly Vector3 OriginalBeamPosition = new Vector3(-0.00146591f, 3.579122f, 10.98136f);

        [Header("Wheels")]
        public bool UseCustomFrontAxle;
        public GameObject? FrontAxle;
        public bool UseCustomDrivers;
        public GameObject? Driver1;
        public GameObject? Driver2;
        public GameObject? Driver3;
        public GameObject? Driver4;
        public bool UseCustomRearAxle;
        public GameObject? RearAxle;

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

        public override IEnumerable<GameObject> GetAllPrefabs(bool includeExploded)
        {
            foreach (var item in base.GetAllPrefabs(includeExploded)) yield return item;
            if (LeftWindow != null) yield return LeftWindow;
            if (RightWindow != null) yield return RightWindow;
            if (ToolboxLid != null) yield return ToolboxLid;
            if (includeExploded && LeftWindowExploded != null) yield return LeftWindowExploded;
            if (includeExploded && RightWindowExploded != null) yield return RightWindowExploded;
            if (includeExploded && ToolboxLidExploded != null) yield return ToolboxLidExploded;
        }

        public override bool DoValidation(out string error)
        {
            error = string.Empty;
            return true;
        }

        public static bool CanCombine(LocoS282730AConfig a, LocoS282730AConfig b) =>
            CarWithInteriorConfig.CanCombine(a, b) &&
            !(a.UseCustomFrontAxle && b.UseCustomFrontAxle) &&
            !(a.UseCustomDrivers && b.UseCustomDrivers) &&
            !(a.UseCustomRearAxle && b.UseCustomRearAxle) &&
            !(a.HideOriginalWindows && b.HideOriginalWindows) &&
            !(a.HideOriginalToolboxLid && !b.HideOriginalToolboxLid) &&
            !(a.UseCustomHeadlights && b.UseCustomHeadlights) &&
            (a.BlockedRegions & b.BlockedRegions) == 0;
    }
}
