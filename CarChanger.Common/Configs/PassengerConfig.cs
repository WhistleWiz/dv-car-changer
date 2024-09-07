using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/Passenger Coach Modification", order = Constants.MenuOrderConstants.Unpowered + 1)]
    public class PassengerConfig : CarWithBogiesConfig
    {
        // Passenger gets a separate one from the rest of the wagons
        // to allow changing the interior.
        [Header("Other")]
        [Tooltip("The coach liveries this modification will apply to")]
        public PassengerType Livery = PassengerType.All;
        [Tooltip("Whether to hide the original interior or not")]
        public bool HideOriginalInterior = false;

        public static bool CanCombine(PassengerConfig a, PassengerConfig b) =>
            CarWithBogiesConfig.CanCombine(a, b) &&
            !(a.HideOriginalInterior && b.HideOriginalInterior);
    }
}
