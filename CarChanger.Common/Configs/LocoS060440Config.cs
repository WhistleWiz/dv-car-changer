﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/S060-440 Modification", order = Constants.MenuOrderConstants.Steam + 0)]
    public class LocoS060440Config : CarWithInteriorConfig
    {
        [Flags]
        public enum ChangeRegions
        {
            None                = 0,
            FrontBufferBeam     = 1 << 0,
            RearBufferBeam      = 1 << 1,
            SmokeDeflectors     = 1 << 2,
            SmokeboxDoor        = 1 << 3,
            Chimney             = 1 << 4,
            Cylinders           = 1 << 5,
            SideTanks           = 1 << 6,
            FrontSandDome       = 1 << 7,
            RearSandDome        = 1 << 8,
            SteamDome           = 1 << 9,
            Firebox             = 1 << 10,
            Bunker              = 1 << 11,
            
            Cab                 = 1 << 27,
            CabRoof             = 1 << 28,
            Whistle             = 1 << 29,
            Bell                = 1 << 30,
            Buffers             = 1 << 31,
        }

        [Serializable]
        public class HeadlightSettings
        {
            [JsonIgnore]
            public static HeadlightSettings Front => new HeadlightSettings
            {
                HighBeamPosition = new Vector3(0.0f, 3.36f, 3.88f),
                LowBeamPosition = new Vector3(0.0f, 1.402f, 4.006f),

                TopGlarePosition = new Vector3(-0.00146591f, 3.354f, 3.883f),
                LeftGlarePosition = new Vector3(-0.894f, 1.402f, 4.028f),
                RightGlarePosition = new Vector3(0.894f, 1.402f, 4.028f)
            };

            [JsonIgnore]
            public static HeadlightSettings Rear => new HeadlightSettings
            {
                HighBeamPosition = new Vector3(0.0f, 3.113f, -4.313f),
                LowBeamPosition = new Vector3(0.0f, 1.556f, -4.216f),

                TopGlarePosition = new Vector3(0.0f, 3.136f, -4.317f),
                LeftGlarePosition = new Vector3(-1.018f, 1.568f, -4.233f),
                RightGlarePosition = new Vector3(1.018f, 1.568f, -4.233f)
            };

            [JsonIgnore]
            public Mesh HighBeamMesh = null!;
            [JsonIgnore]
            public Mesh LowBeamMesh = null!;

            public Vector3 HighBeamPosition;
            public Vector3 LowBeamPosition;

            public Vector3 TopGlarePosition;
            public Vector3 LeftGlarePosition;
            public Vector3 RightGlarePosition;
        }

        [Header("Wheels")]
        public bool UseCustomDrivers;
        public GameObject? Driver1;
        public GameObject? Driver2;
        public GameObject? Driver3;

        [Header("Doors and Windows")]
        public GameObject? DoorLeft;
        public GameObject? DoorRight;
        public GameObject? DoorLeftExploded;
        public GameObject? DoorRightExploded;
        public bool HideOriginalDoors = false;
        public GameObject? WindowLeft;
        public GameObject? WindowRight;
        public GameObject? WindowLeftExploded;
        public GameObject? WindowRightExploded;
        public bool HideOriginalWindows = false;
        public GameObject? Sunroof;
        public GameObject? SunroofExploded;
        public bool HideOriginalSunroof = false;

        [Header("Resources")]
        public GameObject? WaterHatchLeft;
        public GameObject? WaterHatchRight;
        public GameObject? WaterHatchLeftExploded;
        public GameObject? WaterHatchRightExploded;
        public bool HideOriginalHatches = false;
        [EnableIf(nameof(HideOriginalHatches)), Tooltip("The colliders of where water is received on the left side (for servicing)")]
        public GameObject? WaterAreaCollidersLeft;
        [EnableIf(nameof(HideOriginalHatches)), Tooltip("The colliders of where water is received on the right side (for servicing)")]
        public GameObject? WaterAreaCollidersRight;
        [Tooltip("The water object will scale from (1, 0, 1) to (1, 114, 0.45), so beware large objects\n" +
            "Adjust your prefab so it looks correct at those scales")]
        public GameObject? WaterLeft;
        public GameObject? WaterRight;
        public bool HideOriginalWater = false;
        [Tooltip("Replace the coal models with your own\n" +
            "Each model will be enabled based on the percentage of available coal\n" +
            "The percentage array should have 1 less item than the models array, as 0 is assumed as the first value")]
        public bool ReplaceCoal = false;
        [EnableIf(nameof(ReplaceCoal)), Tooltip("In ascending order\nAt 0% all models are hidden")]
        public GameObject[] CoalModels = new GameObject[0];
        [EnableIf(nameof(ReplaceCoal)), Tooltip("In ascending order\n0 does not need to be included")]
        public float[] SwitchPercentage = new float[0];
        [EnableIf(nameof(ReplaceCoal)), Tooltip("The colliders of where coal is received (for servicing)")]
        public GameObject? CoalAreaColliders;

        [Header("Headlights")]
        public bool UseCustomFrontHeadlights = false;
        [EnableIf(nameof(UseCustomFrontHeadlights))]
        public HeadlightSettings FrontSettings = HeadlightSettings.Front;
        [Button(nameof(ResetFrontHeadlights), "Reset"), SerializeField]
        private bool _resetFrontButton;

        public bool UseCustomRearHeadlights = false;
        [EnableIf(nameof(UseCustomRearHeadlights))]
        public HeadlightSettings RearSettings = HeadlightSettings.Rear;
        [Button(nameof(ResetRearHeadlights), "Reset"), SerializeField]
        private bool _resetRearButton;

        [Header("Change Regions"), Tooltip("Which areas does this change block\n" +
            "Other changes with the same areas selected are marked as incompatible with this one")]
        public ChangeRegions BlockedRegions = ChangeRegions.None;

        #region Serialization

        [SerializeField, HideInInspector]
        private string? _frontHeadlights = null;
        [SerializeField, HideInInspector]
        private string? _rearHeadlights = null;
        [SerializeField, HideInInspector]
        private Mesh? _fh = null;
        [SerializeField, HideInInspector]
        private Mesh? _fl = null;
        [SerializeField, HideInInspector]
        private Mesh? _rh = null;
        [SerializeField, HideInInspector]
        private Mesh? _rl = null;

        #endregion

        private void ResetFrontHeadlights() => FrontSettings = HeadlightSettings.Front;

        private void ResetRearHeadlights() => RearSettings = HeadlightSettings.Rear;

        public override void AfterLoad()
        {
            base.AfterLoad();

            _frontHeadlights.FromJson(ref FrontSettings);
            _rearHeadlights.FromJson(ref RearSettings);

            FrontSettings.HighBeamMesh = _fh!;
            FrontSettings.LowBeamMesh = _fl!;
            RearSettings.HighBeamMesh = _rh!;
            RearSettings.LowBeamMesh = _rl!;
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();

            _frontHeadlights = FrontSettings.ToJson();
            _rearHeadlights = RearSettings.ToJson();

            _fh = FrontSettings.HighBeamMesh;
            _fl = FrontSettings.LowBeamMesh;
            _rh = RearSettings.HighBeamMesh;
            _rl = RearSettings.LowBeamMesh;
        }

        public override IEnumerable<GameObject> GetAllPrefabs(bool includeExploded)
        {
            foreach (var item in base.GetAllPrefabs(includeExploded)) yield return item;
            if (Driver1 != null) yield return Driver1;
            if (Driver2 != null) yield return Driver2;
            if (Driver3 != null) yield return Driver3;
            if (DoorLeft != null) yield return DoorLeft;
            if (DoorRight != null) yield return DoorRight;
            if (WindowLeft != null) yield return WindowLeft;
            if (WindowRight != null) yield return WindowRight;
            if (Sunroof != null) yield return Sunroof;
            if (WaterHatchLeft != null) yield return WaterHatchLeft;
            if (WaterHatchRight != null) yield return WaterHatchRight;
            if (includeExploded && DoorLeftExploded != null) yield return DoorLeftExploded;
            if (includeExploded && DoorRightExploded != null) yield return DoorRightExploded;
            if (includeExploded && WindowLeftExploded != null) yield return WindowLeftExploded;
            if (includeExploded && WindowRightExploded != null) yield return WindowRightExploded;
            if (includeExploded && SunroofExploded != null) yield return SunroofExploded;
            if (includeExploded && WaterHatchLeftExploded != null) yield return WaterHatchLeftExploded;
            if (includeExploded && WaterHatchRightExploded != null) yield return WaterHatchRightExploded;
        }

        public override bool DoValidation(out string error)
        {
            if (ReplaceCoal)
            {
                if (CoalModels.Length != SwitchPercentage.Length + 1)
                {
                    error = "number of coal models should be 1 larger than the number of switch percentages";
                    return false;
                }

                if (SwitchPercentage.Length == 0)
                {
                    error = "need at least 1 change percentage";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        public static bool CanCombine(LocoS060440Config a, LocoS060440Config b) =>
            CarWithInteriorConfig.CanCombine(a, b) &&
            !(a.UseCustomDrivers && b.UseCustomDrivers) &&
            !(a.HideOriginalDoors && b.HideOriginalDoors) &&
            !(a.HideOriginalWindows && b.HideOriginalWindows) &&
            !(a.HideOriginalSunroof && b.HideOriginalSunroof) &&
            !(a.HideOriginalHatches && b.HideOriginalHatches) &&
            !(a.HideOriginalWater && b.HideOriginalWater) &&
            !(a.ReplaceCoal && b.ReplaceCoal) &&
            !(a.UseCustomFrontHeadlights && b.UseCustomFrontHeadlights) &&
            !(a.UseCustomRearHeadlights && b.UseCustomRearHeadlights) &&
            (a.BlockedRegions & b.BlockedRegions) == 0;
    }
}
