using UnityEngine;

namespace CarChanger.Common.Configs
{
    [CreateAssetMenu(menuName = "DVCarChanger/Modification Group", order = Constants.MenuOrderConstants.Other + 1)]
    public class ModificationGroupConfig : ModelConfig
    {
        public ModelConfig[] ModificationsToActivate = new ModelConfig[0];

        public override bool DoValidation(out string error)
        {
            foreach (var item in ModificationsToActivate)
            {
                if (item == null)
                {
                    error = $"Empty modification in group!";
                    return false;
                }

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

            bool isWagon = ModificationsToActivate[0] is WagonConfig;
            bool isCCL = ModificationsToActivate[0] is CustomCarConfig;

            // Test if all combinations work together.
            for (int i = 0; i < ModificationsToActivate.Length - 1; i++)
            {
                for (int j = 1; j < ModificationsToActivate.Length; j++)
                {
                    var a = ModificationsToActivate[i];
                    var b = ModificationsToActivate[j];

                    if (!CanCombine(a, b))
                    {
                        error = $"Modifications {a.ModificationId} and {b.ModificationId} cannot be combined!";
                        return false;
                    }

                    if (isWagon && !WagonConfig.SameTargets((WagonConfig)a, (WagonConfig)b))
                    {
                        error = $"Modifications {a.ModificationId} and {b.ModificationId} are targetting different wagons!";
                        return false;
                    }

                    if (isCCL && !CustomCarConfig.SameTargets((CustomCarConfig)a, (CustomCarConfig)b))
                    {
                        error = $"Modifications {a.ModificationId} and {b.ModificationId} are targetting different custom cars!";
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
