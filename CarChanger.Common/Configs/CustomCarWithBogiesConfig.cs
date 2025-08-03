using System.Collections.Generic;
using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/Custom Car With Bogies Modification", order = Constants.MenuOrderConstants.Other + 2)]
    public class CustomCarWithBogiesConfig : CustomCarConfig
    {
        [Header("Bogies")]
        public bool UseCustomBogies = false;
        [EnableIf(nameof(UseCustomBogies))]
        public float WheelRadius = Constants.Wheels.RadiusDefault;
        [EnableIf(nameof(UseCustomBogies))]
        public GameObject? FrontBogie;
        [EnableIf(nameof(UseCustomBogies))]
        public GameObject? RearBogie;
        public bool Powered = false;

        public override IEnumerable<GameObject> GetAllPrefabs(bool includeExploded)
        {
            foreach (var item in base.GetAllPrefabs(includeExploded)) yield return item;
            if (FrontBogie != null) yield return FrontBogie;
            if (RearBogie != null) yield return RearBogie;
        }

        public override bool DoValidation(out string error)
        {
            if (FrontBogie || RearBogie)
            {
                var result = Validation.ValidateBothBogies(FrontBogie, RearBogie);

                if (!string.IsNullOrEmpty(result))
                {
                    error = result!;
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        public static bool CanCombine(CustomCarWithBogiesConfig a, CustomCarWithBogiesConfig b) =>
            !(a.UseCustomBogies && b.UseCustomBogies);
    }
}
