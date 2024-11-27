using Newtonsoft.Json;
using System;
using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/DM1U-150 (Utility Rail Vehicle) Modification", order = Constants.MenuOrderConstants.Diesel + 5)]
    public class LocoDM1U150Config : CarWithInteriorAndBogiesConfig
    {
        [Serializable]
        public class HeadlightSettings
        {
            [JsonIgnore]
            public static HeadlightSettings Front => new HeadlightSettings
            {
                HighBeamPosition = new Vector3(0.0f, 1.535f, 6.736f),
                LowBeamPosition = new Vector3(0.0f, 1.535f, 6.736f),

                TopGlarePosition = new Vector3(0.0f, 2.928f, 6.496f),
                LeftGlarePosition = new Vector3(-0.881f, 1.535f, 6.736f),
                RightGlarePosition = new Vector3(0.881f, 1.535f, 6.736f),

                RedLeftGlarePosition = new Vector3(-0.881f, 1.339f, 6.667f),
                RedRightGlarePosition = new Vector3(0.881f, 1.339f, 6.667f)
            };

            [JsonIgnore]
            public static HeadlightSettings Rear => new HeadlightSettings
            {
                HighBeamPosition = new Vector3(0.0f, 2.084f, -6.678f),
                LowBeamPosition = new Vector3(0.0f, 2.084f, -6.678f),

                TopGlarePosition = new Vector3(0.0f, 0.0f, 0.0f),
                LeftGlarePosition = new Vector3(-1.329f, 2.084f, -6.675f),
                RightGlarePosition = new Vector3(1.329f, 2.084f, -6.675f),

                RedLeftGlarePosition = new Vector3(-1.329f, 1.882f, -6.675f),
                RedRightGlarePosition = new Vector3(1.329f, 1.882f, -6.675f)
            };

            [JsonIgnore]
            public Mesh? WhiteTopMesh = null;
            [JsonIgnore]
            public Mesh? WhiteLowMesh = null;
            [JsonIgnore]
            public Mesh? RedMesh = null;

            public Vector3 HighBeamPosition;
            public Vector3 LowBeamPosition;

            public Vector3 TopGlarePosition;
            public Vector3 LeftGlarePosition;
            public Vector3 RightGlarePosition;

            public Vector3 RedLeftGlarePosition;
            public Vector3 RedRightGlarePosition;
        }

        protected override float OriginalRadius => Constants.Wheels.RadiusDM1P;

        [Header("Doors and Windows")]
        public GameObject? DoorRear;
        public bool HideOriginalDoors = false;
        public GameObject? DoorInterior;
        public bool HideOriginalInteriorDoor = false;

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

        [Header("Other")]
        [EnableIf(nameof(HideOriginalInterior)), Tooltip("Only active if hiding the interior is also active")]
        public bool HideDash = false;

        #region Serialization

        [SerializeField, HideInInspector]
        private string? _frontHeadlights = null;
        [SerializeField, HideInInspector]
        private string? _rearHeadlights = null;
        [SerializeField, HideInInspector]
        private Mesh? _fwh = null;
        [SerializeField, HideInInspector]
        private Mesh? _fwl = null;
        [SerializeField, HideInInspector]
        private Mesh? _fr = null;
        [SerializeField, HideInInspector]
        private Mesh? _rwh = null;
        [SerializeField, HideInInspector]
        private Mesh? _rwl = null;
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

            FrontSettings.WhiteTopMesh = _fwh;
            FrontSettings.WhiteLowMesh = _fwl;
            FrontSettings.RedMesh = _fr;
            RearSettings.WhiteTopMesh = _rwh;
            RearSettings.WhiteLowMesh = _rwl;
            RearSettings.RedMesh = _rr;
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();

            _frontHeadlights = FrontSettings.ToJson();
            _rearHeadlights = RearSettings.ToJson();

            // Can't really serialize the meshes in the json so here they go.
            _fwh = FrontSettings.WhiteTopMesh;
            _fwl = FrontSettings.WhiteLowMesh;
            _fr = FrontSettings.RedMesh;
            _rwh = RearSettings.WhiteTopMesh;
            _rwh = RearSettings.WhiteLowMesh;
            _rr = RearSettings.RedMesh;
        }

        public override bool DoValidation(out string error)
        {
            if (FrontBogie || RearBogie)
            {
                var result = Validation.ValidateBothBogies(FrontBogie, RearBogie, 1, 1, 1, 0);

                if (!string.IsNullOrEmpty(result))
                {
                    error = result!;
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        public static bool CanCombine(LocoDM1U150Config a, LocoDM1U150Config b) =>
            CarWithInteriorAndBogiesConfig.CanCombine(a, b) &&
            !(a.HideOriginalDoors && b.HideOriginalDoors) &&
            !(a.HideOriginalInteriorDoor && b.HideOriginalInteriorDoor) &&
            !(a.UseCustomFrontHeadlights && b.UseCustomFrontHeadlights) &&
            !(a.UseCustomRearHeadlights && b.UseCustomRearHeadlights);
    }
}
