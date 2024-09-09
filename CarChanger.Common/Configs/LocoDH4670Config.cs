using Newtonsoft.Json;
using System;
using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/DH4-670 Modification", order = Constants.MenuOrderConstants.Diesel + 2)]
    public class LocoDH4670Config : CarWithInteriorAndBogiesConfig
    {
        [Serializable]
        public class HeadlightSettings
        {
            [JsonIgnore]
            public static HeadlightSettings Front => new HeadlightSettings
            {
                HighBeamPosition = new Vector3(0.0f, 3.9342f, 1.0234f),
                LowBeamPosition = new Vector3(0.0f, 1.4577f, 6.16f),

                TopGlarePosition = new Vector3(0.0f, 3.9342f, 1.008f),
                LeftGlarePosition = new Vector3(-0.52f, 1.4577f, 6.1342f),
                RightGlarePosition = new Vector3(0.52f, 1.4577f, 6.1342f),

                RedLeftGlarePosition = new Vector3(-0.795f, 1.4577f, 6.0825f),
                RedRightGlarePosition = new Vector3(0.795f, 1.4577f, 6.0825f)
            };

            [JsonIgnore]
            public static HeadlightSettings Rear => new HeadlightSettings
            {
                HighBeamPosition = new Vector3(0.0f, 3.9342f, -5.394f),
                LowBeamPosition = new Vector3(0.0f, 1.4577f, -6.16f),

                TopGlarePosition = new Vector3(0.0f, 3.034f, -5.41f),
                LeftGlarePosition = new Vector3(-0.519f, 1.4577f, -6.1342f),
                RightGlarePosition = new Vector3(0.52f, 1.4577f, -6.1342f),

                RedLeftGlarePosition = new Vector3(-0.7929f, 1.4577f, -6.0825f),
                RedRightGlarePosition = new Vector3(0.7932f, 1.4577f, -6.0825f)
            };

            [JsonIgnore]
            public Mesh? WhiteHighBeamMesh = null;
            [JsonIgnore]
            public Mesh? WhiteLowBeamMesh = null;
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

        protected override float OriginalRadius => Constants.Wheels.RadiusDE6;

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

            FrontSettings.WhiteHighBeamMesh = _fwh;
            FrontSettings.WhiteLowBeamMesh = _fwl;
            FrontSettings.RedMesh = _fr;
            RearSettings.WhiteHighBeamMesh = _rwh;
            RearSettings.WhiteLowBeamMesh = _rwl;
            RearSettings.RedMesh = _rr;
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();

            _frontHeadlights = FrontSettings.ToJson();
            _rearHeadlights = RearSettings.ToJson();

            // Can't really serialize the meshes in the json so here they go.
            _fwh = FrontSettings.WhiteHighBeamMesh;
            _fwl = FrontSettings.WhiteLowBeamMesh;
            _fr = FrontSettings.RedMesh;
            _rwh = RearSettings.WhiteHighBeamMesh;
            _rwh = RearSettings.WhiteLowBeamMesh;
            _rr = RearSettings.RedMesh;
        }

        public override bool DoValidation(out string error)
        {
            if (FrontBogie || RearBogie)
            {
                var result = Validation.ValidateBothBogies(FrontBogie, RearBogie, 2, 2, 2, 2);

                if (!string.IsNullOrEmpty(result))
                {
                    error = result!;
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        public static bool CanCombine(LocoDH4670Config a, LocoDH4670Config b) =>
            CarWithInteriorAndBogiesConfig.CanCombine(a, b) &&
            !(a.HideOriginalDoors && b.HideOriginalDoors) &&
            !(a.HideOriginalWindows && b.HideOriginalWindows) &&
            !(a.UseCustomFrontHeadlights && b.UseCustomFrontHeadlights) &&
            !(a.UseCustomRearHeadlights && b.UseCustomRearHeadlights);
    }
}
