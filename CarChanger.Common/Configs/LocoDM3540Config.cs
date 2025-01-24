using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/DM3-540 Modification", order = Constants.MenuOrderConstants.Diesel + 4)]
    public class LocoDM3540Config : CarWithInteriorConfig
    {
        [Serializable]
        public class HeadlightSettings
        {
            [JsonIgnore]
            public static HeadlightSettings Front => new HeadlightSettings
            {
                BeamPosition = new Vector3(0.0f, 1.4006f, 3.7522f),

                LeftGlarePosition = new Vector3(-0.855f, 1.407f, 3.736f),
                RightGlarePosition = new Vector3(0.848f, 1.407f, 3.739f),

                RedGlarePosition = new Vector3(-0.005f, 3.101f, 3.33f)
            };

            [JsonIgnore]
            public static HeadlightSettings Rear => new HeadlightSettings
            {
                BeamPosition = new Vector3(0.0f, 1.7786f, -4.0382f),

                LeftGlarePosition = new Vector3(0.852f, 1.762f, -3.8996f),
                RightGlarePosition = new Vector3(-0.8451f, 1.762f, -3.897f),

                RedGlarePosition = new Vector3(0.0f, 3.104f, -3.904f)
            };

            [JsonIgnore]
            public Mesh WhiteMesh = null!;
            [JsonIgnore]
            public Mesh RedMesh = null!;

            public Vector3 BeamPosition;

            public Vector3 LeftGlarePosition;
            public Vector3 RightGlarePosition;

            public Vector3 RedGlarePosition;
        }

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

        [Header("Other")]
        [EnableIf(nameof(HideOriginalInterior)), Tooltip("Only active if hiding the interior is also active")]
        public bool HideGearPlaque = false;

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
            if (DoorLeft != null) yield return DoorLeft;
            if (DoorRight != null) yield return DoorRight;
            if (WindowLeft != null) yield return WindowLeft;
            if (WindowRight != null) yield return WindowRight;
            if (includeExploded && DoorLeftExploded != null) yield return DoorLeftExploded;
            if (includeExploded && DoorRightExploded != null) yield return DoorRightExploded;
            if (includeExploded && WindowLeftExploded != null) yield return WindowLeftExploded;
            if (includeExploded && WindowRightExploded != null) yield return WindowRightExploded;
        }

        public override bool DoValidation(out string error)
        {
            error = string.Empty;
            return true;
        }

        public static bool CanCombine(LocoDM3540Config a, LocoDM3540Config b) =>
            CarWithInteriorConfig.CanCombine(a, b) &&
            !(a.HideOriginalDoors && b.HideOriginalDoors) &&
            !(a.HideOriginalWindows && b.HideOriginalWindows) &&
            !(a.UseCustomFrontHeadlights && b.UseCustomFrontHeadlights) &&
            !(a.UseCustomRearHeadlights && b.UseCustomRearHeadlights);
    }
}
