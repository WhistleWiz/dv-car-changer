using CarChanger.Common;
using DV.CabControls;
using DV.CabControls.Spec;
using DV.Interaction;
using DV.ThingTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CarChanger.Game
{
    public static class Helpers
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

        public static void RefreshIndicator(Indicator indicator)
        {
            var t = indicator.GetType();
            var m = t.GetMethod("OnValueSet", BindingFlags.Instance | BindingFlags.NonPublic);
            m.Invoke(indicator, null);
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

        public static GameObject CreateEmptyInactiveObject(string name, Transform parent = null!)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.SetActive(false);
            return go;
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

        public static void DestroyIfNotNull(UnityEngine.Object? obj)
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

        public static void MoveControl(ControlSpec control, Vector3 move)
        {
            var joint = control.GetComponent<Joint>();

            if (joint == null)
            {
                CarChangerMod.Error($"Tried to move control {control.name}, but joint has not been created yet!");
                return;
            }

            MoveControl(control, joint, move);
        }

        internal static void MoveControl(ControlSpec control, Joint joint, Vector3 move)
        {
            // Reset the control to 0, preventing weird stacking of angles as you apply and revert changes.
            var cBase = control.GetComponent<ControlImplBase>();
            cBase?.SetValue(0);

            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor += move;

            // Get the interaction area if it exists.
            // It isn't part of the base ControlSpec, so reflection it is...
            var t = control.GetType();
            var f = t.GetField("nonVrStaticInteractionArea");

            if (f != null)
            {
                var ia = (StaticInteractionArea)f.GetValue(control);

                if (ia != null)
                {
                    ia.transform.localPosition += move;
                }
            }
        }

        internal static StaticInteractionArea? GetInteractionArea(ControlSpec control)
        {
            var t = control.GetType();
            var f = t.GetField("nonVrStaticInteractionArea");

            if (f == null) return null;
            return (StaticInteractionArea)f.GetValue(control);
        }
    }
}
