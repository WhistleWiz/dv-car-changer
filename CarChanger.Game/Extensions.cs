using CarChanger.Game.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game
{
    public static class Extensions
    {
        public static bool TryFind<T>(this List<T> list, Predicate<T> match, out T value)
        {
            value = list.Find(match);

            if (value == null)
            {
                return false;
            }

            return true;
        }

        public static void ForceRefreshLoadedPrefabs(this TrainCar car)
        {
            if (car.IsInteriorLoaded)
            {
                car.UnloadInterior();
                car.LoadInterior();
            }

            if (car.AreExternalInteractablesLoaded)
            {
                car.UnloadExternalInteractables();
                car.LoadExternalInteractables();
            }
            else if (car.AreDummyExternalInteractablesLoaded)
            {
                car.UnloadDummyExternalInteractables();
                car.LoadDummyExternalInteractables();
            }
        }

        internal static List<AppliedChange> GetAppliedChanges(this TrainCar car)
        {
            var comps = car.GetComponents<AppliedChange>();
            var changes = new List<AppliedChange>();

            foreach (var change in comps)
            {
                if (change != null)
                {
                    if (change.Config == null)
                    {
                        UnityEngine.Object.Destroy(change);
                    }
                    else
                    {
                        changes.Add(change);
                    }
                }
            }

            return changes;
        }

        public static IEnumerable<GameObject> AllChildGOs(this Transform t)
        {
            foreach (Transform child in t)
            {
                yield return child.gameObject;
            }
        }

        public static IEnumerable<GameObject> AllChildGOsExcept(this Transform t, params string[] names)
        {
            foreach (var item in t.AllChildGOs())
            {
                if (names.Contains(item.name)) continue;

                yield return item;
            }
        }

        public static void ResetLocal(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }
    }
}
