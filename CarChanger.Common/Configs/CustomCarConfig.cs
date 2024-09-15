using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/Custom Car Modification", order = Constants.MenuOrderConstants.Other + 1)]
    public class CustomCarConfig : ModelConfig
    {
        [Tooltip("The car type this modification will apply to\n" +
            "Leaving it empty will instead apply to the livery")]
        public string CarTypeId = string.Empty;
        [EnableIf(nameof(EnableLivery)), Tooltip("The livery this modification will apply to")]
        public string LiveryId = string.Empty;

        [Tooltip("The prefab to load on the body")]
        public GameObject BodyPrefab = null!;

        [Header("Colliders")]
        [Tooltip("The colliders of the car with the world")]
        public GameObject? CollisionCollider;
        [Tooltip("The colliders of the car with the player")]
        public GameObject? WalkableCollider;
        [Tooltip("The colliders of the car with items")]
        public GameObject? ItemsCollider;

        [Header("Interior")]
        [Tooltip("The prefab to load on the interior\n" +
            "Only the static parts of the interior will be affected, all controls will remain as is")]
        public GameObject? InteriorPrefab;
        [Tooltip("The prefab to load on the exploded interior\n" +
            "Only the static parts of the interior will be affected, all controls will remain as is")]
        public GameObject? InteriorPrefabExploded;

        [Header("Interior LOD")]
        [Tooltip("The prefab to load on the interior LOD\n" +
            "This is the part of the interior shown while the interior itself is unloaded")]
        public GameObject? InteriorLODPrefab;

        protected bool EnableLivery() => string.IsNullOrEmpty(CarTypeId);

        public override bool DoValidation(out string error)
        {
            error = string.Empty;
            return true;
        }

        public static bool SameTargets(CustomCarConfig a, CustomCarConfig b)
        {
            if (a.CarTypeId != b.CarTypeId)
            {
                return false;
            }

            if (a.EnableLivery())
            {
                return a.LiveryId == b.LiveryId;
            }

            return true;
        }
    }
}
