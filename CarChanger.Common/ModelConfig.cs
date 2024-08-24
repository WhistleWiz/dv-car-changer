using CarChanger.Common.Configs;
using DVLangHelper.Data;
using Newtonsoft.Json;
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

        public event Action<ModelConfig, GameObject>? OnConfigApplied;
        public event Action<ModelConfig, GameObject>? OnConfigUnapplied;

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
                case LocoDE6Config de6A:
                    if (b is LocoDE6Config de6B) return LocoDE6Config.CanCombine(de6A, de6B);
                    else return false;
                default:
                    break;
            }

            // If somehow it's not a valid combination, fail.
            return false;
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
    }
}
