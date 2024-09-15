using CarChanger.Common.Components;
using CarChanger.Game.Components;
using DV.CabControls.Spec;
using DV.Rain;
using System.Linq;
using UnityEngine;

namespace CarChanger.Game
{
    internal class ComponentProcessor
    {
        public static void ProcessComponents(GameObject gameObject, MaterialHolder holder)
        {
            if (gameObject == null) return;

            if (gameObject.transform.parent != null)
            {
                gameObject.SetLayersRecursive(gameObject.transform.parent.gameObject.layer);
            }

            // Find the root car or interior transform.
            Transform? root = null;
            var interior = gameObject.GetComponentInParent<TrainCarInteriorObject>();

            if (interior != null)
            {
                root = interior.transform;
            }
            else
            {
                var car = TrainCar.Resolve(gameObject);

                if (car != null)
                {
                    root = car.transform;
                }
            }

            ProcessDefaultMaterial(gameObject, holder);
            ProcessMoveThisControl(gameObject);
            ProcessWindows(gameObject);

            if (root != null)
            {
                ProcessHideTransforms(gameObject, root);
                ProcessMoveTransforms(gameObject, root);
            }
        }

        private static void ProcessDefaultMaterial(GameObject gameObject, MaterialHolder holder)
        {
            foreach (var item in gameObject.GetComponentsInChildren<UseDefaultMaterial>())
            {
                item.GetRenderer().material = holder.GetMaterial(item);
            }
        }

        private static void ProcessMoveThisControl(GameObject gameObject)
        {
            if (!gameObject.TryGetComponent(out ChangeThisControl comp)) return;

            var control = comp.GetComponentInParent<ControlSpec>();

            if (control != null)
            {
                ChangeThisControlInternal.Create(comp, control);
            }
            else
            {
                CarChangerMod.Error($"No control found to move on {comp.name}");
            }

            Object.Destroy(comp);
        }

        private static void ProcessWindows(GameObject gameObject)
        {
            // Check if there's an existing window.
            var window = gameObject.GetComponentInParent<Window>();

            if (window == null)
            {
                window = gameObject.GetComponentInSiblings<Window>();

                if (window == null)
                {
                    window = gameObject.GetComponentInChildren<Window>(true);

                    if (window == null) return;
                }
            }

            // Deactivate the current object to prevent Awake() from being called too early.
            gameObject.SetActive(false);
            var dupes = window.duplicates.ToList();

            foreach (var item in gameObject.GetComponentsInChildren<WindowSetup>(true))
            {
                // Initialise even the empty arrays, or they'll be null.
                var dupe = item.gameObject.AddComponent<Window>();
                dupe.visuals = item.Renderers;
                dupe.sizeInMeters = item.Size;
                dupe.wipers = new Wiper[0];
                dupe.duplicates = new Window[0];
                dupe.windowEdges = new Transform[0];
                dupes.Add(dupe);

                // Register this window duplicate for removal when it is destroyed.
                item.DestroyEvent += () =>
                {
                    var list = window.duplicates.ToList();
                    list.Remove(dupe);
                    window.duplicates = list.ToArray();
                };
            }

            window.duplicates = dupes.ToArray();
            gameObject.SetActive(true);
        }

        public static void ProcessHideTransforms(GameObject gameObject, Transform root)
        {
            foreach (var item in gameObject.GetComponentsInChildren<HideTransformsOnChange>())
            {
                item.Hide(root);
            }
        }

        public static void ProcessMoveTransforms(GameObject gameObject, Transform root)
        {
            foreach (var item in gameObject.GetComponentsInChildren<MoveTransformsOnChange>())
            {
                item.Apply(root);
            }
        }

        public static void ProcessTeleportPassThroughColliders(GameObject gameObject)
        {
            foreach (var item in gameObject.GetComponentsInChildren<TeleportPassthroughCollider>())
            {
                var comp = item.gameObject.AddComponent<TeleportArcPassThrough>();
                comp.twoSided = item.TwoSided;
                comp.colliders = item.OtherColliders.ToHashSet();
            }
        }
    }
}
