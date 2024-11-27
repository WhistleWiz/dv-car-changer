using CarChanger.Common.Components;
using CarChanger.Game.Components;
using DV;
using DV.CabControls.Spec;
using DV.Customization.Gadgets;
using DV.Rain;
using DV.Simulation.Cars;
using LocoSim.Implementations;
using System.Collections.Generic;
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

            ProcessDefaultMaterial(gameObject, holder);
            ProcessMoveThisControl(gameObject);
            ProcessWindows(gameObject);
            ProcessBlockResourceReceivers(gameObject);
            ProcessCustomizationMeshes(gameObject);

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

            // If a root was found, process these types of components.
            if (root != null)
            {
                ProcessDuplicateTransforms(gameObject, root);
                ProcessHideTransforms(gameObject, root);
                ProcessMoveTransforms(gameObject, root);
            }

            // Similar to the root but for the car simulation.
            var simController = holder.Car.GetComponent<SimController>();

            if (simController != null)
            {
                ProcessPortValueToRotations(gameObject, simController.SimulationFlow);
                ProcessPortValueToAnimators(gameObject, simController.SimulationFlow);
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

                //CoroutineManager.Instance.Run(WindowFixRoutine(window));
            }

            window.duplicates = dupes.ToArray();
            gameObject.SetActive(true);
        }

        //private static System.Collections.IEnumerator WindowFixRoutine(Window window)
        //{
        //    yield return null;
        //    yield return null;

        //    if (window != null)
        //    {

        //    }
        //}

        private static void ProcessBlockResourceReceivers(GameObject gameObject)
        {
            foreach (var item in gameObject.GetComponentsInChildren<BlockResourceReceivers>())
            {
                item.gameObject.AddComponent<InteriorNonStandardLayer>();
                item.gameObject.layer = 15;
            }
        }

        private static void ProcessPortValueToRotations(GameObject gameObject, SimulationFlow flow)
        {
            foreach (var item in gameObject.GetComponentsInChildren<PortValueToRotation>())
            {
                if (flow.TryGetPort(item.PortId, out var port))
                {
                    item.ValueGetter = () => port.Value;
                }
                else
                {
                    CarChangerMod.Error($"Could not find port ID '{item.PortId}' for PortValueToRotation '{item.name}'");
                }
            }
        }

        private static void ProcessPortValueToAnimators(GameObject gameObject, SimulationFlow flow)
        {
            foreach (var item in gameObject.GetComponentsInChildren<PortValueToAnimation>())
            {
                if (flow.TryGetPort(item.PortId, out var port))
                {
                    item.ValueGetter = () => port.Value;
                }
                else
                {
                    CarChangerMod.Error($"Could not find port ID '{item.PortId}' for PortValueToAnimation '{item.name}'");
                }
            }
        }

        public static void ProcessDuplicateTransforms(GameObject gameObject, Transform root)
        {
            foreach (var item in gameObject.GetComponentsInChildren<DuplicateTransformsOnChange>())
            {
                item.Duplicate(root);
            }
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

        public static void ProcessCustomizationMeshes(GameObject gameObject)
        {
            var meshes = gameObject.GetComponentInChildren<CustomizationPlacementMeshes>();

            if (meshes == null) return;

            var car = TrainCar.Resolve(gameObject);

            if (car == null) return;

            var real = car.GetComponent<DV.Customization.CustomizationPlacementMeshes>();

            if (real == null) return;

            var root = Helpers.GetCustomizationRoot(car, real);
            int layer = Layers.DVLayer.Gadget_Mesh_Placing.ToInt();

            List<GameObject> colliders = new List<GameObject>();

            foreach (var item in meshes.CollisionMeshes)
            {
                colliders.Add(Create(item));
            }

            foreach (var item in meshes.DrillDisableMeshes)
            {
                colliders.Add(Create(item).AddComponent<DrillingDisabler>().gameObject);
            }

            meshes.ClearListOnDestroy = () =>
            {
                foreach (var item in colliders)
                {
                    Object.Destroy(item);
                }
            };

            GameObject Create(MeshFilter filter)
            {
                GameObject obj = new GameObject("[GadgetMeshCollider][" + filter.name + "]");
                obj.transform.SetParent(root.holder, worldPositionStays: false);
                Vector3 position = meshes.transform.InverseTransformPoint(filter.transform.position);
                Quaternion rotation = Quaternion.Inverse(meshes.transform.rotation) * filter.transform.rotation;
                obj.transform.position = root.holder.TransformPoint(position);
                obj.transform.rotation = root.holder.rotation * rotation;
                obj.layer = layer;
                obj.AddComponent<MeshCollider>().sharedMesh = filter.sharedMesh;
                return obj;
            }
        }
    }
}
