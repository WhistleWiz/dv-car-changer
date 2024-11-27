using CarChanger.Common;
using DV;
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
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

        private static Dictionary<string, TrainCarType_v2>? s_idToCarType;
        private static Dictionary<WagonType, string> s_carTypeEnumToId = new Dictionary<WagonType, string>()
        {
            { WagonType.Autorack, "Autorack" },
            { WagonType.Boxcar, "Boxcar" },
            { WagonType.BoxcarMilitary, "BoxcarMilitary" },
            { WagonType.Flatbed, "Flatbed" },
            { WagonType.FlatbedMilitary, "FlatbedMilitary" },
            { WagonType.FlatbedStakes, "FlatbedStakes" },
            { WagonType.FlatbedShort, "FlatbedShort" },
            { WagonType.Gondola, "Gondola" },
            { WagonType.Hopper, "Hopper" },
            { WagonType.HopperCovered, "HopperCovered" },
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
            () => Globals.G.Types.carTypes.ToDictionary(x => x.id, x => x));

        public static T GetCached<T>(ref T? cacheValue, Func<T> getter) where T : class
        {
            cacheValue ??= getter();
            return cacheValue;
        }

        public static void InvalidateBogieCache(Bogie b)
        {
            var t = typeof(Bogie);
            var f = t.GetField("axles", Flags);
            f.SetValue(b, null);
        }

        public static void RefreshIndicator(IndicatorModelChanger indicator)
        {
            var t = typeof(IndicatorModelChanger);
            var f = t.GetField("currentModelIndex", BindingFlags.Instance | BindingFlags.NonPublic);
            f.SetValue(indicator, -1);

            RefreshIndicator((Indicator)indicator);
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

        public static void DestroyGameObjectIfNotNull(Component? obj)
        {
            if (obj != null)
            {
                UnityEngine.Object.Destroy(obj.gameObject);
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

        public static void ChangeSteppedJointLimits(HingeJoint joint, SteppedJoint stepped)
        {
            var t = typeof(SteppedJoint);
            var range = joint.useLimits ? (joint.limits.max - joint.limits.min) : 360f;

            t.GetField("angleRange", Flags).SetValue(stepped, range);
            t.GetProperty(nameof(stepped.SingleNotchAngle)).SetValue(stepped, range / (stepped.notches - (joint.useLimits ? 1 : 0)));

            var a = AngleForNotch(stepped.innerLimitMinNotch);
            var b = AngleForNotch(stepped.innerLimitMaxNotch);
            t.GetField("innerLimitMinAngle", Flags).SetValue(stepped, Mathf.Min(a, b));
            t.GetField("innerLimitMaxAngle", Flags).SetValue(stepped, Mathf.Max(a, b));

            // Avoid calling the original one as this may run before Start() is called on that script.
            float AngleForNotch(int notch) => stepped.invertDirection ?
                joint.limits.max - notch * stepped.SingleNotchAngle :
                joint.limits.min + notch * stepped.SingleNotchAngle;
        }

        public static Transform? ReplaceInteractionPoint(ControlSpec control, Transform replacePoint)
        {
            if (control.TryGetComponent(out PointHandSnapper point))
            {
                var t = point.pointMarker;
                point.pointMarker = replacePoint;
                return t;
            }

            if (control.TryGetComponent(out LineHandSnapper line))
            {
                var t = line.lineStart;
                line.lineStart = replacePoint;
                return t;
            }

            if (control.TryGetComponent(out ValveHandSnapper valve))
            {
                var t = valve.axis;
                valve.axis = replacePoint;
                return t;
            }

            if (control.TryGetComponent(out CircleHandSnapper circle))
            {
                var t = circle.centerUpward;
                circle.centerUpward = replacePoint;
                return t;
            }

            return null;
        }

        public static DV.Customization.CustomizationPlacementMeshes.GadgetColliderHolder GetCustomizationRoot(TrainCar car,
            DV.Customization.CustomizationPlacementMeshes meshes)
        {
            var m = typeof(DV.Customization.CustomizationPlacementMeshes).GetMethod("FindRoot", Flags);

            return (DV.Customization.CustomizationPlacementMeshes.GadgetColliderHolder)m.Invoke(meshes, new object[] { car });
        }
    }
}
