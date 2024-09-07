using UnityEngine;

namespace CarChanger.Common.Configs
{
    public class CabooseConfig : CarWithInteriorAndBogiesConfig
    {
        [Header("Doors and Windows")]
        public GameObject? DoorFront;
        public GameObject? DoorRear;
        public bool HideOriginalDoors = false;

        public static bool CanCombine(CabooseConfig a, CabooseConfig b) =>
            CarWithInteriorAndBogiesConfig.CanCombine(a, b) &&
            !(a.HideOriginalDoors && b.HideOriginalDoors);
    }
}
