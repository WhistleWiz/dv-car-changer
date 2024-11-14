using CarChanger.Common.Configs;
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
        [Tooltip("Forces prefabs to be reloaded when this modification is applied/unapplied\n" +
            "Has a performance cost, so avoid if not needed\n" +
            "Can be useful if the modification wants to change controls")]
        public bool ForcePrefabReloadOnApply = false;

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
            // Don't stack the same.
            if (a == b)
            {
                return false;
            }

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
                case CabooseConfig cabooseA:
                    if (b is CabooseConfig cabooseB) return CabooseConfig.CanCombine(cabooseA, cabooseB);
                    else return false;

                case LocoDE2480Config de2480A:
                    if (b is LocoDE2480Config de2480B) return LocoDE2480Config.CanCombine(de2480A, de2480B);
                    else return false;
                case LocoDE6860Config de6860A:
                    if (b is LocoDE6860Config de6860B) return LocoDE6860Config.CanCombine(de6860A, de6860B);
                    else return false;

                case LocoDH4670Config dh4670A:
                    if (b is LocoDH4670Config dh4670B) return LocoDH4670Config.CanCombine(dh4670A, dh4670B);
                    else return false;

                case LocoDM3540Config dm3540A:
                    if (b is LocoDM3540Config dm3540B) return LocoDM3540Config.CanCombine(dm3540A, dm3540B);
                    else return false;

                case LocoS060440Config s060440A:
                    if (b is LocoS060440Config s060440B) return LocoS060440Config.CanCombine(s060440A, s060440B);
                    else return false;
                case LocoS282730AConfig s282730AA:
                    if (b is LocoS282730AConfig s282730AB) return LocoS282730AConfig.CanCombine(s282730AA, s282730AB);
                    else return false;
                case LocoS282730BConfig s282730BA:
                    if (b is LocoS282730BConfig s282730BB) return LocoS282730BConfig.CanCombine(s282730BA, s282730BB);
                    else return false;

                case LocoBE2260Config be2260A:
                    if (b is LocoBE2260Config be2260B) return LocoBE2260Config.CanCombine(be2260A, be2260B);
                    else return false;

                case LocoDE6860SlugConfig de6860SlugA:
                    if (b is LocoDE6860SlugConfig de6860SlugB) return LocoDE6860SlugConfig.CanCombine(de6860SlugA, de6860SlugB);
                    else return false;
                case LocoHandcarConfig handcarA:
                    if (b is LocoHandcarConfig handcarB) return CarWithBogiesConfig.CanCombine(handcarA, handcarB);
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
