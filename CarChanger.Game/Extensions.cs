﻿using System;
using System.Collections.Generic;

namespace CarChanger.Game
{
    internal static class Extensions
    {
        public static List<AppliedChange> GetAppliedChanges(this TrainCar car)
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

        public static bool TryFind<T>(this List<T> list, Predicate<T> match, out T value)
        {
            value = list.Find(match);

            if (value == null)
            {
                return false;
            }

            return true;
        }
    }
}
