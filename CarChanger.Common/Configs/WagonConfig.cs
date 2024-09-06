using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/Freight Wagon Modification", order = Constants.MenuOrderConstants.Unpowered + 0)]
    public class WagonConfig : CarWithBogiesConfig
    {
        [Header("Other")]
        [Tooltip("The car type this modification will apply to\n" +
            "'Use Livery' applies it to a single livery instead of all liveries of a type")]
        public WagonType CarType = WagonType.Flatbed;
        [EnableIf(nameof(EnableLivery)), Tooltip("The livery this modification will apply to")]
        public WagonLivery CarLivery = WagonLivery.FlatbedEmpty;

        private bool EnableLivery() => CarType == WagonType.UseLivery;

        public static bool CanCombine(WagonConfig a, WagonConfig b)
        {
            return CarWithBogiesConfig.CanCombine(a, b);
        }

        public static bool SameTargets(WagonConfig a, WagonConfig b)
        {
            if (a.CarType != b.CarType)
            {
                return false;
            }

            if (a.CarType == WagonType.UseLivery)
            {
                return a.CarLivery == b.CarLivery;
            }

            return true;
        }
    }
}
