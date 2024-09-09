using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/Handcar Modification", order = Constants.MenuOrderConstants.Extras + 1)]
    public class LocoHandcarConfig : CarWithBogiesConfig
    {
        protected override float OriginalRadius => Constants.Wheels.RadiusHandcar;

        public override bool DoValidation(out string error)
        {
            if (FrontBogie || RearBogie)
            {
                var result = Validation.ValidateBothBogies(FrontBogie, RearBogie, null, null, null, null, 1);

                if (!string.IsNullOrEmpty(result))
                {
                    error = result!;
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }
    }
}
