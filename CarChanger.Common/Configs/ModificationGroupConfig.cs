using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/Modification Group")]
    public class ModificationGroupConfig : ModelConfig
    {
        public ModelConfig[] ModificationsToActivate = new ModelConfig[0];

        public override bool DoValidation(out string error)
        {
            foreach (var item in ModificationsToActivate)
            {
                if (this == item)
                {
                    error = $"Cannot include self in group!";
                    return false;
                }

                if (item is ModificationGroupConfig)
                {
                    error = $"Cannot include groups in other groups ({item.ModificationId})!";
                    return false;
                }
            }

            // Test if all combinations work together.
            for (int i = 0; i < ModificationsToActivate.Length - 1; i++)
            {
                for (int j = 1; j < ModificationsToActivate.Length; j++)
                {
                    if (!CanCombine(ModificationsToActivate[i], ModificationsToActivate[j]))
                    {
                        error = $"Modifications {ModificationsToActivate[i].ModificationId} and {ModificationsToActivate[j]} cannot be combined!";
                        return false;
                    }
                }
            }

            error = string.Empty;
            return true;
        }

        public static bool CanCombine(ModificationGroupConfig a, ModelConfig b)
        {
            // Can't really combine groups directly anyways.
            if (b is ModificationGroupConfig groupB)
            {
                return false;
            }

            // Check if each modification of the group can be combined.
            foreach (var item in a.ModificationsToActivate)
            {
                if (!CanCombine(item, b))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
