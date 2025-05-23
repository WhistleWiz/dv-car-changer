﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/DE6-860 Modification", order = Constants.MenuOrderConstants.Diesel + 1)]
    public class LocoDE6860Config : CarWithInteriorAndBogiesConfig
    {
        [Flags]
        public enum ChangeRegions
        {
            None                = 0,
            ShortNose           = 1 << 0,
            LongNose            = 1 << 1,
            FrontRailings       = 1 << 2,
            SideRailings        = 1 << 3,
            RearRailings        = 1 << 4,
            Fans                = 1 << 5,
            Exhaust             = 1 << 6,
            FuelTanks           = 1 << 7,
            NumberBoards        = 1 << 8,

            ControlStand        = 1 << 15,
            Speedometer         = 1 << 16,
            CabHeater           = 1 << 17,
            Table               = 1 << 18,
            ToiletDoor          = 1 << 19,
            Fridge              = 1 << 20,
            CabLight            = 1 << 21,

            Cab                 = 1 << 27,
            CabRoof             = 1 << 28,
            Horn                = 1 << 29,
            //Bell                = 1 << 30,
            Buffers             = 1 << 31,
        }

        [Serializable]
        public class HeadlightSettings
        {
            [JsonIgnore]
            public static HeadlightSettings Front => new HeadlightSettings
            {
                HighBeamPosition = new Vector3(0.0f, 3.6f, 8.925f),
                LowBeamPosition = new Vector3(0.0f, 1.99f, 8.925f),

                TopGlarePosition = new Vector3(0.0f, 3.759f, 7.222f),
                LeftGlarePosition = new Vector3(-0.6184f, 2.0064f, 8.41f),
                RightGlarePosition = new Vector3(0.6184f, 2.0064f, 8.41f),

                RedGlarePosition = new Vector3(0.0f, 2.833f, 8.529f)
            };

            [JsonIgnore]
            public static HeadlightSettings Rear => new HeadlightSettings
            {
                HighBeamPosition = new Vector3(0.0f, 3.6f, -8.914f),
                LowBeamPosition = new Vector3(0.0f, 1.99f, -8.914f),

                TopGlarePosition = new Vector3(0.0f, 3.449f, -8.28f),
                LeftGlarePosition = new Vector3(0.6184f, 2.0064f, -8.27f),
                RightGlarePosition = new Vector3(-0.6184f, 2.0064f, -8.27f),

                RedGlarePosition = new Vector3(0.0f, 3.121f, -8.32f)
            };

            [JsonIgnore]
            public Mesh WhiteMesh = null!;
            [JsonIgnore]
            public Mesh RedMesh = null!;

            public Vector3 HighBeamPosition;
            public Vector3 LowBeamPosition;

            public Vector3 TopGlarePosition;
            public Vector3 LeftGlarePosition;
            public Vector3 RightGlarePosition;

            public Vector3 RedGlarePosition;
        }

        protected override float OriginalRadius => Constants.Wheels.RadiusDE6;

        [Header("Doors and Windows")]
        public GameObject? EngineDoorLeft;
        public GameObject? EngineDoorRight;
        public GameObject? EngineDoorLeftExploded;
        public GameObject? EngineDoorRightExploded;
        public bool HideOriginalEngineDoors = false;
        public GameObject? CabDoorFront;
        public GameObject? CabDoorRear;
        public GameObject? CabDoorFrontExploded;
        public GameObject? CabDoorRearExploded;
        public bool HideOriginalCabDoors = false;

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
        private Mesh? _fw = null;
        [SerializeField, HideInInspector]
        private Mesh? _fr = null;
        [SerializeField, HideInInspector]
        private Mesh? _rw = null;
        [SerializeField, HideInInspector]
        private Mesh? _rr = null;

        #endregion

        private void ResetFrontHeadlights() => FrontSettings = HeadlightSettings.Front;

        private void ResetRearHeadlights() => RearSettings = HeadlightSettings.Rear;

        public override void AfterLoad()
        {
            base.AfterLoad();

            _frontHeadlights.FromJson(ref FrontSettings);
            _rearHeadlights.FromJson(ref RearSettings);

            FrontSettings.WhiteMesh = _fw!;
            FrontSettings.RedMesh = _fr!;
            RearSettings.WhiteMesh = _rw!;
            RearSettings.RedMesh = _rr!;
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();

            _frontHeadlights = FrontSettings.ToJson();
            _rearHeadlights = RearSettings.ToJson();

            // Can't really serialize the meshes in the json so here they go.
            _fw = FrontSettings.WhiteMesh;
            _fr = FrontSettings.RedMesh;
            _rw = RearSettings.WhiteMesh;
            _rr = RearSettings.RedMesh;
        }

        public override IEnumerable<GameObject> GetAllPrefabs(bool includeExploded)
        {
            foreach (var item in base.GetAllPrefabs(includeExploded)) yield return item;
            if (CabDoorFront != null) yield return CabDoorFront;
            if (CabDoorRear != null) yield return CabDoorRear;
            if (EngineDoorLeft != null) yield return EngineDoorLeft;
            if (EngineDoorRight != null) yield return EngineDoorRight;
            if (includeExploded && CabDoorFrontExploded != null) yield return CabDoorFrontExploded;
            if (includeExploded && CabDoorRearExploded != null) yield return CabDoorRearExploded;
            if (includeExploded && EngineDoorLeftExploded != null) yield return EngineDoorLeftExploded;
            if (includeExploded && EngineDoorRightExploded != null) yield return EngineDoorRightExploded;
        }

        public override bool DoValidation(out string error)
        {
            if (FrontBogie || RearBogie)
            {
                var result = Validation.ValidateBothBogies(FrontBogie, RearBogie, 3, 3, 3, 3);

                if (!string.IsNullOrEmpty(result))
                {
                    error = result!;
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        public static bool CanCombine(LocoDE6860Config a, LocoDE6860Config b) =>
            CarWithInteriorAndBogiesConfig.CanCombine(a, b) &&
            !(a.HideOriginalEngineDoors && b.HideOriginalEngineDoors) &&
            !(a.HideOriginalCabDoors && b.HideOriginalCabDoors) &&
            !(a.UseCustomFrontHeadlights && b.UseCustomFrontHeadlights) &&
            !(a.UseCustomRearHeadlights && b.UseCustomRearHeadlights) &&
            (a.BlockedRegions & b.BlockedRegions) == 0;
    }
}
