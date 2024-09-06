﻿using CarChanger.Common.Configs;
using DVLangHelper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarChanger.Common
{
    public abstract class ModelConfig : ScriptableObject, ISerializationCallbackReceiver
    {
        [Header("Modification Settings")]
        [Tooltip("The UNIQUE ID for your modification")]
        public string ModificationId = string.Empty;
        [Tooltip("The name of your modification")]
        public TranslationData ModificationName = new TranslationData();
        [Tooltip("Specific modification IDs that can't be used with this one")]
        public string[] IncompatibleModifications = new string[0];

        [SerializeField, HideInInspector]
        private string? _jsonName = null;

        /// <summary>
        /// Called when this config is applied. Receives itself and the <see cref="GameObject"/> of the car.
        /// </summary>
        public event Action<ModelConfig, GameObject>? OnConfigApplied;
        /// <summary>
        /// Called when this config is unapplied. Receives itself and the <see cref="GameObject"/> of the car.
        /// </summary>
        public event Action<ModelConfig, GameObject>? OnConfigUnapplied;

        /// <summary>
        /// Called when this config is applied to the interior. This may happen multiple times while the outside is only applied once,
        /// as the interior loads and unloads. Receives itself, the interior <see cref="GameObject"/> of a car, and whether or not it is exploded.
        /// </summary>
        public event Action<ModelConfig, GameObject, bool>? OnInteriorApplied;
        /// <summary>
        /// Called when this config is unapplied to the interior. This is only called when the interior is active and the config is unapplied.
        /// It will not be called when unloading the interior. Receives itself and the interior <see cref="GameObject"/> of a car.
        /// </summary>
        public event Action<ModelConfig, GameObject>? OnInteriorUnapplied;

        /// <summary>
        /// Called when this config is applied to the external interactables.
        /// Receives itself, the interactables <see cref="GameObject"/> of a car, and whether or not it is exploded.
        /// </summary>
        public event Action<ModelConfig, GameObject, bool>? OnInteractablesApplied;
        /// <summary>
        /// Called when this config is unapplied to the external interactables.
        /// Receives itself and the interactables <see cref="GameObject"/> of a car.
        /// </summary>
        public event Action<ModelConfig, GameObject>? OnInteractablesUnapplied;

        public string LocalizationKey => $"carchanger/{ModificationId.ToLower()}";

        public virtual void OnBeforeSerialize()
        {
            _jsonName = ModificationName.ToJson();
        }

        public virtual void OnAfterDeserialize() { }

        public virtual void AfterLoad()
        {
            _jsonName.FromJson(ref ModificationName);
        }

        public abstract bool DoValidation(out string error);

        public static bool CanCombine(ModelConfig a, ModelConfig b)
        {
            // If the modifications are incompatible return right away.
            if (a.IncompatibleModifications.Contains(b.ModificationId) ||
                b.IncompatibleModifications.Contains(a.ModificationId))
            {
                return false;
            }

            // Special case if the 2nd one is a group, same effect as swapping a and b.
            // Shorts if both are groups, in which case it returns false right away.
            if (b is ModificationGroupConfig groupB)
            {
                return ModificationGroupConfig.CanCombine(groupB, a);
            }

            // Keep testing for more specific combinations.
            switch (a)
            {
                case ModificationGroupConfig groupA:
                    return ModificationGroupConfig.CanCombine(groupA, b);
                case WagonConfig wagonA:
                    if (b is WagonConfig wagonB) return WagonConfig.CanCombine(wagonA, wagonB);
                    else return false;
                case PassengerConfig paxA:
                    if (b is PassengerConfig paxB) return PassengerConfig.CanCombine(paxA, paxB);
                    else return false;
                case LocoDE6Config de6A:
                    if (b is LocoDE6Config de6B) return LocoDE6Config.CanCombine(de6A, de6B);
                    else return false;
                case LocoS282AConfig s282A:
                    if (b is LocoS282AConfig s282B) return LocoDE6Config.CanCombine(s282A, s282B);
                    else return false;
                case CustomCarConfig _:
                    if (b is CustomCarConfig) return true;
                    else return false;
                default:
                    // If the types didn't match, fail.
                    return false;
            }
        }

        public static bool CanCombine(ModelConfig a, IEnumerable<ModelConfig> others)
        {
            foreach (var item in others)
            {
                if (!CanCombine(a, item))
                {
                    return false;
                }
            }

            return true;
        }

        public void Applied(GameObject gameObject)
        {
            OnConfigApplied?.Invoke(this, gameObject);
        }

        public void Unapplied(GameObject gameObject)
        {
            OnConfigUnapplied?.Invoke(this, gameObject);
        }

        public void InteriorApplied(GameObject gameObject, bool isExploded)
        {
            OnInteriorApplied?.Invoke(this, gameObject, isExploded);
        }

        public void InteriorUnapplied(GameObject gameObject)
        {
            OnInteriorUnapplied?.Invoke(this, gameObject);
        }

        public void InteractablesApplied(GameObject gameObject, bool isExploded)
        {
            OnInteractablesApplied?.Invoke(this, gameObject, isExploded);
        }

        public void InteractablesUnapplied(GameObject gameObject)
        {
            OnInteractablesUnapplied?.Invoke(this, gameObject);
        }
    }
}
