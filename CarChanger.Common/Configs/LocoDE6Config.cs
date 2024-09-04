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

        [Header("Body")]
        [Tooltip("The prefab to load on the body")]
        public GameObject? BodyPrefab = null;
        [Tooltip("Whether to hide the original body or not")]
        public bool HideOriginalBody = false;

        [Header("Colliders")]
        [Tooltip("The colliders of the car with the world")]
        public GameObject? CollisionCollider = null;
        [Tooltip("The colliders of the car with the player")]
        public GameObject? WalkableCollider = null;
        [Tooltip("The colliders of the car with items")]
        public GameObject? ItemsCollider = null;

        [Header("Interior")]
        [Tooltip("The prefab to load on the cab\n" +
            "Only the static parts of the cab will be affected, all controls will remain as is")]
        public GameObject? CabStaticPrefab = null;
        [Tooltip("The prefab to load on the exploded cab\n" +
            "Only the static parts of the cab will be affected, all controls will remain as is")]
        public GameObject? CabStaticPrefabExploded = null;
        [Tooltip("Whether to hide the original cab or not\n" +
            "Only the static parts of the cab will be affected, all controls will remain as is")]
        public bool HideOriginalCab = false;

        [Header("Doors and Windows")]
        public GameObject? EngineDoorLeft = null;
        public GameObject? EngineDoorRight = null;
        public GameObject? EngineDoorLeftExploded = null;
        public GameObject? EngineDoorRightExploded = null;
        public bool HideOriginalEngineDoors = false;
        public GameObject? CabDoorFront = null;
        public GameObject? CabDoorRear = null;
        public GameObject? CabDoorFrontExploded = null;
        public GameObject? CabDoorRearExploded = null;
        public bool HideOriginalCabDoors = false;

        [Header("Bogies")]
        public bool UseCustomBogies = false;
        [EnableIf(nameof(EnableBogies))]
        public float WheelRadius = Constants.WheelRadiusDE6;
        [EnableIf(nameof(EnableBogies))]
        public GameObject? FrontBogie = null!;
        [EnableIf(nameof(EnableBogies))]
        public GameObject? RearBogie = null!;

        [Header("Headlights")]
        public bool UseCustomFrontHeadlights = false;
        [EnableIf(nameof(EnableFrontHeadlights))]
        public HeadlightSettings FrontSettings = HeadlightSettings.Front;
        public bool UseCustomRearHeadlights = false;
        [EnableIf(nameof(EnableRearHeadlights))]
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
        /// as the interior loads and unloads. Receives itself, the interior <see cref="GameObject"/> of a car, and whether or not it is exploded.
        /// </summary>
        public event Action<LocoDE6Config, GameObject, bool>? OnInteriorApplied;
        /// <summary>
        /// Called when this config is unapplied to the interior. This is only called when the interior is active and the config is unapplied.
        /// It will not be called when unloading the interior. Receives itself and the interior <see cref="GameObject"/> of a car.
        /// </summary>
        public event Action<LocoDE6Config, GameObject>? OnInteriorUnapplied;

        /// <summary>
        /// Called when this config is applied to the external interactables.
        /// Receives itself, the interactables <see cref="GameObject"/> of a car, and whether or not it is exploded.
        /// </summary>
        public event Action<LocoDE6Config, GameObject, bool>? OnInteractablesApplied;
        /// <summary>
        /// Called when this config is unapplied to the external interactables.
        /// Receives itself and the interactables <see cref="GameObject"/> of a car.
        /// </summary>
        public event Action<LocoDE6Config, GameObject>? OnInteractablesUnapplied;

        private void Reset()
        {
            ResetBogies();
            ResetFrontHeadlights();
            ResetRearHeadlights();
        }

        public void ResetBogies()
        {
            UseCustomBogies = false;
            WheelRadius = 0.5335f;
            FrontBogie = null!;
            RearBogie = null!;
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
            if (FrontBogie || RearBogie)
            {
                var result = Validation.ValidateBothBogies(FrontBogie, RearBogie, null, null, 3, 3);

                if (!string.IsNullOrEmpty(result))
                {
                    error = result!;
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        public void InteriorApplied(GameObject gameObject, bool isExploded)
        {
            OnInteriorApplied?.Invoke(this, gameObject, isExploded);
        }

        public void InteriorUnapplied(GameObject gameObject)
        {
            OnInteriorUnapplied?.Invoke(this, gameObject);
        }

        public void InteractablesApplied(GameObject gameObject, bool isExploded)
        {
            OnInteractablesApplied?.Invoke(this, gameObject, isExploded);
        }

        public void InteractablesUnapplied(GameObject gameObject)
        {
            OnInteractablesUnapplied?.Invoke(this, gameObject);
        }

        private bool EnableBogies() => UseCustomBogies;
        private bool EnableFrontHeadlights() => UseCustomFrontHeadlights;
        private bool EnableRearHeadlights() => UseCustomRearHeadlights;

        public static bool CanCombine(LocoDE6Config a, LocoDE6Config b)
        {
            return !(a.UseCustomBogies && b.UseCustomBogies) &&
                !(a.HideOriginalBody && b.HideOriginalBody) &&
                !(a.HideOriginalCab && b.HideOriginalCab) &&
                !(a.HideOriginalEngineDoors && b.HideOriginalEngineDoors) &&
                !(a.HideOriginalCabDoors && b.HideOriginalCabDoors) &&
                !(a.UseCustomFrontHeadlights && b.UseCustomFrontHeadlights) &&
                !(a.UseCustomRearHeadlights && b.UseCustomRearHeadlights);
        }
    }
}
