﻿using UnityEngine;

namespace CarChanger.Common.Configs
{
    public class LocoS282BConfig : CarWithBogiesConfig
    {
        private static readonly Vector3 OriginalBeamPosition = new Vector3(0.0024f, 3.2164f, -7.9304f);

        [Header("Resources")]
        public GameObject? WaterHatch;
        public bool HideOriginalHatch = false;
        [Tooltip("The water object will scale from (1, 0, 1) to (1, 145, 1), so beware large objects\n" +
            "Adjust your prefab so it looks correct at those scales")]
        public GameObject? Water;
        public bool HideOriginalWater = false;
        [Tooltip("Replace the coal models with your own\n" +
            "Each model will be enabled based on the percentage of available coal\n" +
            "The percentage array should have 1 less item than the models array, as 0 is assumed as the first value")]
        public bool ReplaceCoal = false;
        [EnableIf(nameof(EnableCoal)), Tooltip("In ascending order\nAt 0% all models are hidden")]
        public GameObject[] CoalModels = new GameObject[0];
        [EnableIf(nameof(EnableCoal)), Tooltip("In ascending order\n0 does not need to be included")]
        public float[] SwitchPercentage = new float[0];

        [Header("Headlights")]
        public bool UseCustomHeadlights = false;
        [EnableIf(nameof(EnableHeadlights))]
        public Mesh Mesh = null!;
        [EnableIf(nameof(EnableHeadlights))]
        public Vector3 BeamPosition = OriginalBeamPosition;
        [Button(nameof(ResetHeadlights), "Reset Position"), SerializeField]
        private bool _resetHeadlight;

        public override bool DoValidation(out string error)
        {
            if (!base.DoValidation(out error)) return false;

            if (ReplaceCoal)
            {
                if (CoalModels.Length != SwitchPercentage.Length + 1)
                {
                    error = "number of coal models should be 1 larger than the number of switch percentages";
                    return false;
                }
            }

            return true;
        }

        private void ResetHeadlights()
        {
            BeamPosition = OriginalBeamPosition;
        }

        private bool EnableHeadlights() => UseCustomHeadlights;

        private bool EnableCoal() => ReplaceCoal;

        public static bool CanCombine(LocoS282BConfig a, LocoS282BConfig b) =>
            CarWithBogiesConfig.CanCombine(a, b) &&
            !(a.UseCustomHeadlights && b.UseCustomHeadlights);
    }
}
