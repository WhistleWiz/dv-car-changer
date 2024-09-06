using UnityEngine;

namespace CarChanger.Common.Configs
{
    public class CabooseConfig : CarWithInteriorAndBogiesConfig
    {
        [Header("Doors and Windows")]
        public GameObject? DoorFront = null;
        public GameObject? DoorRear = null;
        public bool HideOriginalDoors = false;

        public static bool CanCombine(CabooseConfig a, CabooseConfig b)
        {
            return CarWithInteriorAndBogiesConfig.CanCombine(a, b) &&
                !(a.HideOriginalDoors && b.HideOriginalDoors);
        }
    }
}
