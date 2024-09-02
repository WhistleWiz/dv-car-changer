using CarChanger.Common.Components;
using UnityEngine;

namespace CarChanger.Common
{
    internal static class Validation
    {
        public static string? ValidateBogie(GameObject bogie, int? requiredAxles = null, int? requiredPoweredAxles = null)
        {
            int axles = 0;
            int poweredAxles = 0;

            foreach (Transform t in bogie.transform)
            {
                if (t.name == "[axle]")
                {
                    axles++;

                    if (t.TryGetComponent<PoweredAxle>(out _))
                    {
                        poweredAxles++;
                    }
                }
            }

            if (axles < 1)
            {
                return "no axles detected in bogie";
            }

            if (requiredAxles.HasValue && axles != requiredAxles)
            {
                return $"number of axles ({axles}) is different from expected ({requiredAxles})";
            }

            if (requiredPoweredAxles.HasValue && poweredAxles != requiredPoweredAxles)
            {
                return $"number of powered axles ({poweredAxles}) is different from expected ({requiredPoweredAxles})";
            }

            return null;
        }

        public static string? ValidateBothBogies(GameObject front, GameObject rear,
            int? requiredAxlesF = null, int? requiredAxlesR = null,
            int? requiredPoweredAxlesF = null, int? requiredPoweredAxlesR = null)
        {
            if (front)
            {
                if (!rear)
                {
                    return "must set either both bogies or none";
                }
            }
            else if (rear)
            {
                return "must set either both bogies or none";
            }

            if (front)
            {
                var result = ValidateBogie(front, requiredAxlesF, requiredPoweredAxlesF);

                if (!string.IsNullOrEmpty(result))
                {
                    return $"front bogie: {result}";
                }
            }

            if (rear)
            {
                var result = ValidateBogie(rear, requiredAxlesR, requiredPoweredAxlesR);

                if (!string.IsNullOrEmpty(result))
                {
                    return $"rear bogie: {result}";
                }
            }

            return null;
        }
    }
}
