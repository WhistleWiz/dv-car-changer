﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/DE2-480 Modification", order = Constants.MenuOrderConstants.Diesel + 0)]
    public class LocoDE2480Config : CarWithInteriorAndBogiesConfig
    {
        [Serializable]
        public class HeadlightSettings
        {
            [JsonIgnore]
            public static HeadlightSettings Front => new HeadlightSettings
            {
                BeamPosition = new Vector3(0.0f, 1.521f, 3.269f),

                LeftGlarePosition = new Vector3(-0.6467f, 1.5274f, 3.27f),
                RightGlarePosition = new Vector3(0.6467f, 1.5274f, 3.27f),

                RedGlarePosition = new Vector3(-0.005f, 2.799f, 3.237f)
            };

            [JsonIgnore]
            public static HeadlightSettings Rear => new HeadlightSettings
            {
                BeamPosition = new Vector3(0.0f, 3.6f, -8.914f),

                LeftGlarePosition = new Vector3(-0.6467f, 1.9274f, -3.27f),
                RightGlarePosition = new Vector3(0.6467f, 1.9274f, -3.27f),

                RedGlarePosition = new Vector3(0.005f, 2.458f, -3.267f)
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

        protected override float OriginalRadius => Constants.Wheels.RadiusDE2480;

        [Header("Doors and Windows")]
        public GameObject? DoorFront;
        public GameObject? DoorRear;
        public GameObject? DoorFrontExploded;
        public GameObject? DoorRearExploded;
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

        [Header("Other")]
        [EnableIf(nameof(HideOriginalInterior)), Tooltip("Only active if hiding the interior is also active")]
        public bool HideControlDeck = false;

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

            _fw = FrontSettings.WhiteMesh;
            _fr = FrontSettings.RedMesh;
            _rw = RearSettings.WhiteMesh;
            _rr = RearSettings.RedMesh;
        }

        public override IEnumerable<GameObject> GetAllPrefabs(bool includeExploded)
        {
            foreach (var item in base.GetAllPrefabs(includeExploded)) yield return item;
            if (DoorFront != null) yield return DoorFront;
            if (DoorRear != null) yield return DoorRear;
            if (includeExploded && DoorFrontExploded != null) yield return DoorFrontExploded;
            if (includeExploded && DoorRearExploded != null) yield return DoorRearExploded;
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

        public static bool CanCombine(LocoDE2480Config a, LocoDE2480Config b) =>
            CarWithInteriorAndBogiesConfig.CanCombine(a, b) &&
            !(a.HideOriginalCabDoors && b.HideOriginalCabDoors) &&
            !(a.UseCustomFrontHeadlights && b.UseCustomFrontHeadlights) &&
            !(a.UseCustomRearHeadlights && b.UseCustomRearHeadlights);
    }
}
