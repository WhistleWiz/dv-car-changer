﻿using Newtonsoft.Json;
using System;
using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/DE6 Modification")]
    public class LocoDE6Config : ModelConfig
    {
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
                LeftGlarePosition = new Vector3(-0.6184f, 2.0064f, -8.27f),
                RightGlarePosition = new Vector3(0.6184f, 2.0064f, -8.27f),

                RedGlarePosition = new Vector3(0.0f, 3.121f, -8.32f)
            };

            [JsonIgnore]
            public Mesh White = null!;
            [JsonIgnore]
            public Mesh Red = null!;

            public Vector3 HighBeamPosition;
            public Vector3 LowBeamPosition;

            public Vector3 TopGlarePosition;
            public Vector3 LeftGlarePosition;
            public Vector3 RightGlarePosition;

            public Vector3 RedGlarePosition;
        }

        [Tooltip("The prefab to load on the body")]
        public GameObject BodyPrefab = null!;
        [Tooltip("Whether to hide the original body or not")]
        public bool HideOriginalBody = false;
        [Tooltip("The prefab to load on the cab\n" +
            "Only the static parts of the cab will be affected, all controls will remain as is")]
        public GameObject CabStaticPrefab = null!;
        [Tooltip("Whether to hide the original cab or not\n" +
            "Only the static parts of the cab will be affected, all controls will remain as is")]
        public bool HideOriginalCab = false;
        [Tooltip("Moves the entire cab (including controls) by an offset")]
        public Vector3 CabInteriorOffset = Vector3.zero;

        [Header("Bogies")]
        public bool UseCustomBogies = false;
        public float WheelRadius = 0.5335f;
        public GameObject FrontBogie = null!;
        public GameObject RearBogie = null!;

        [Header("Headlights")]
        public bool UseCustomFrontHeadlights = false;
        public HeadlightSettings FrontSettings = HeadlightSettings.Front;
        public bool UseCustomRearHeadlights = false;
        public HeadlightSettings RearSettings = HeadlightSettings.Rear;
        
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

        /// <summary>
        /// Called when this config is applied to the interior. This may happen multiple times while the outside is only applied once,
        /// as the interior loads and unloads. Receives itself and the interior <see cref="GameObject"/> of a car.
        /// </summary>
        public event Action<LocoDE6Config, GameObject>? OnInteriorApplied;
        /// <summary>
        /// Called when this config is unapplied to the interior. This is only called when the interior is active and the config is unapplied.
        /// It will not be called when unloading the interior. Receives itself and the interior <see cref="GameObject"/> of a car.
        /// </summary>
        public event Action<LocoDE6Config, GameObject>? OnInteriorUnapplied;

        private void OnReset()
        {
            ResetFrontHeadlights();
            ResetRearHeadlights();
        }

        public void ResetFrontHeadlights()
        {
            FrontSettings = HeadlightSettings.Front;
        }

        public void ResetRearHeadlights()
        {
            RearSettings = HeadlightSettings.Rear;
        }

        public override void AfterLoad()
        {
            base.AfterLoad();

            _frontHeadlights.FromJson(ref FrontSettings);
            _rearHeadlights.FromJson(ref RearSettings);

            FrontSettings.White = _fw!;
            FrontSettings.Red = _fr!;
            RearSettings.White = _rw!;
            RearSettings.Red = _rr!;
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();

            _frontHeadlights = FrontSettings.ToJson();
            _rearHeadlights = RearSettings.ToJson();

            // Can't really serialize the meshes in the json so here they go.
            _fw = FrontSettings.White;
            _fr = FrontSettings.Red;
            _rw = RearSettings.White;
            _rr = RearSettings.Red;
        }

        public override bool DoValidation(out string error)
        {
            error = string.Empty;
            return true;
        }

        public void InteriorApplied(GameObject gameObject)
        {
            OnInteriorApplied?.Invoke(this, gameObject);
        }

        public void InteriorUnapplied(GameObject gameObject)
        {
            OnInteriorUnapplied?.Invoke(this, gameObject);
        }

        public static bool CanCombine(LocoDE6Config a, LocoDE6Config b)
        {
            return !(a.UseCustomBogies && b.UseCustomBogies) &&
                !(a.HideOriginalBody && b.HideOriginalBody) &&
                !(a.HideOriginalCab && b.HideOriginalCab) &&
                !(a.UseCustomFrontHeadlights && b.UseCustomFrontHeadlights) &&
                !(a.UseCustomRearHeadlights && b.UseCustomRearHeadlights);
        }
    }
}
