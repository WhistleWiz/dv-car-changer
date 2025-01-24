using System.Collections.Generic;
using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/Caboose Modification", order = Constants.MenuOrderConstants.Unpowered + 2)]
    public class CabooseConfig : CarWithInteriorAndBogiesConfig
    {
        [Header("Doors and Windows")]
        public GameObject? DoorFront;
        public GameObject? DoorRear;
        public bool HideOriginalDoors = false;

        public override IEnumerable<GameObject> GetAllPrefabs(bool includeExploded)
        {
            foreach (var item in base.GetAllPrefabs(includeExploded)) yield return item;
            if (DoorFront != null) yield return DoorFront;
            if (DoorRear != null) yield return DoorRear;
        }

        public static bool CanCombine(CabooseConfig a, CabooseConfig b) =>
            CarWithInteriorAndBogiesConfig.CanCombine(a, b) &&
            !(a.HideOriginalDoors && b.HideOriginalDoors);
    }
}
