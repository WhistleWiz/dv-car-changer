using CarChanger.Common;
using DV.ThingTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CarChanger.Game
{
    internal static class Helpers
    {
        private static Dictionary<string, TrainCarType_v2>? s_idToCarType;
        private static Dictionary<WagonType, string> s_carTypeEnumToId = new Dictionary<WagonType, string>()
        {
            { WagonType.Autorack, "Autorack" },
            { WagonType.Boxcar, "Boxcar" },
            { WagonType.BoxcarMilitary, "BoxcarMilitary" },
            { WagonType.Flatbed, "Flatbed" },
            { WagonType.FlatbedMilitary, "FlatbedMilitary" },
            { WagonType.FlatbedStakes, "FlatbedStakes" },
            { WagonType.Gondola, "Gondola" },
            { WagonType.Hopper, "Hopper" },
            { WagonType.NuclearFlask, "NuclearFlask" },
            { WagonType.Stock, "Stock" },
            { WagonType.Refrigerator, "Refrigerator" },
            { WagonType.TankChem, "TankChem" },
            { WagonType.TankGas, "TankGas" },
            { WagonType.TankOil, "TankOil" },
            { WagonType.TankShortFood, "TankShortFood" }
        };

        private static System.Random? s_random;
        public static System.Random RNG => GetCached(ref s_random, () => new System.Random());

        public static Dictionary<string, TrainCarType_v2> IdToCarType => GetCached(ref s_idToCarType,
            () => DV.Globals.G.Types.carTypes.ToDictionary(x => x.id, x => x));

        public static T GetCached<T>(ref T? cacheValue, Func<T> getter) where T : class
        {
            cacheValue ??= getter();
            return cacheValue;
        }

        public static void InvalidateBogieCache(Bogie b)
        {
            var t = typeof(Bogie);
            var f = t.GetField("axles", BindingFlags.Instance | BindingFlags.NonPublic);
            f.SetValue(b, null);
        }

        public static TrainCarType_v2 EnumToCarType(WagonType carType)
        {
            return IdToCarType[s_carTypeEnumToId[carType]];
        }

        public static int Wrap(int i, int max)
        {
            if (i < 0)
            {
                return max - 1;
            }

            if (i >= max)
            {
                return 0;
            }

            return i;
        }

        public static bool Chance(double percent)
        {
            return RNG.NextDouble() < percent;
        }

        public static T InstantiateIfNotNull<T>(T? obj)
            where T : UnityEngine.Object
        {
            return InstantiateIfNotNull(obj, null!);
        }

        public static T InstantiateIfNotNull<T>(T? obj, Transform t)
            where T : UnityEngine.Object
        {
            if (obj == null) return null!;

            return UnityEngine.Object.Instantiate(obj, t);
        }

        public static void DestroyIfNotNull(UnityEngine.Object obj)
        {
            if (obj != null)
            {
                UnityEngine.Object.Destroy(obj);
            }
        }

        public static void SetLayersForAllChildren(GameObject gameObject, int layer)
        {
            foreach (var item in gameObject.GetComponentsInChildren<Transform>())
            {
                item.gameObject.layer = layer;
            }
        }
    }
}
