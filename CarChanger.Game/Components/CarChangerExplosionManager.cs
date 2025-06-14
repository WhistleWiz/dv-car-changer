using CarChanger.Common.Components;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CarChanger.Game.Components
{
    internal class CarChangerExplosionManager : MonoBehaviour
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
        private Transform _explodeParent = null!;

        private void Awake()
        {
            var trainCar = TrainCar.Resolve(gameObject);

            if (trainCar == null)
            {
                CarChangerMod.Error("Could not find train car, destroying self.");
                Destroy(this);
                return;
            }

            _explodeParent = new GameObject("[car changer explosion stuff]").transform;
            _explodeParent.parent = trainCar.transform;
            _explodeParent.ResetLocal();
        }

        public void HandleExplode()
        {
            foreach (var item in _explodeParent.GetComponentsInChildren<ExplosionModelHandler>())
            {
                item.HandleExplosionModelChange();
            }
        }

        public void HandleUnexplode()
        {
            foreach (var item in _explodeParent.GetComponentsInChildren<ExplosionModelHandler>())
            {
                item.RevertToUnexplodedModel();
            }
        }

        public ExplosionModelHandler CreateHandler(GameObject[] disableGameObjects, ExplosionModelHandler.GameObjectSwapData[] gameObjectsSwaps,
            ExplosionModelHandler.MaterialSwapData[] matSwaps)
        {
            var go = Helpers.CreateEmptyInactiveObject("handler", _explodeParent);
            var handler = go.AddComponent<ExplosionModelHandler>();

            var t = typeof(ExplosionModelHandler);
            t.GetField("gameObjectsToDisable", Flags).SetValue(handler, disableGameObjects);
            t.GetField("gameObjectSwaps", Flags).SetValue(handler, gameObjectsSwaps);
            t.GetField("materialSwaps", Flags).SetValue(handler, matSwaps);

            go.SetActive(true);
            return handler;
        }

        private static CarChangerExplosionManager GetOrCreateExplosionManager(TrainCar car)
        {
            if (!car.TryGetComponent(out CarChangerExplosionManager manager))
            {
                manager = car.gameObject.AddComponent<CarChangerExplosionManager>();
            }

            return manager;
        }

        public static ExplosionModelHandler? PrepareExplosionHandler(GameObject gameObject, MaterialHolder holder)
        {
            GetEntries(gameObject, out var disableGos, out var goSwaps, out var matSwaps);
            return PrepareExplosionHandlerFromEntries(holder, disableGos, goSwaps, matSwaps);
        }

        public static ExplosionModelHandler? PrepareExplosionHandlerFromEntries(MaterialHolder holder,
            IEnumerable<DisableGameObjectOnExplosion>? disableGameObjects = null,
            IEnumerable<SwapGameObjectOnExplosion>? gameObjectsSwaps = null,
            IEnumerable<SwapMaterialOnExplosion>? matSwaps = null)
        {

            if ((disableGameObjects == null || disableGameObjects.Count() == 0) &&
                (gameObjectsSwaps == null || gameObjectsSwaps.Count() == 0) &&
                (matSwaps == null || matSwaps.Count() == 0))
            {
                return null;
            }

            disableGameObjects ??= System.Array.Empty<DisableGameObjectOnExplosion>();
            gameObjectsSwaps ??= System.Array.Empty<SwapGameObjectOnExplosion>();
            matSwaps ??= System.Array.Empty<SwapMaterialOnExplosion>();

            var manager = GetOrCreateExplosionManager(holder.Car);

            var handler = manager.CreateHandler(disableGameObjects.Select(x => x.gameObject).ToArray(),
                gameObjectsSwaps.Select(x => ToData(x, holder)).ToArray(),
                matSwaps.Select(x => ToData(x, holder)).ToArray());

            if (holder.Car.isExploded)
            {
                handler.HandleExplosionModelChange();
            }

            return handler;
        }

        public static void GetEntries(GameObject go, out DisableGameObjectOnExplosion[] disableGameObjects,
            out SwapGameObjectOnExplosion[] gameObjectsSwaps,
            out SwapMaterialOnExplosion[] matSwaps)
        {
            disableGameObjects = go.GetComponentsInChildren<DisableGameObjectOnExplosion>();
            gameObjectsSwaps = go.GetComponentsInChildren<SwapGameObjectOnExplosion>();
            matSwaps = go.GetComponentsInChildren<SwapMaterialOnExplosion>();
        }

        private static ExplosionModelHandler.GameObjectSwapData ToData(SwapGameObjectOnExplosion comp, MaterialHolder holder)
        {
            // Instantiate a new prefab instead of using it directly.
            // This allows processing per car, to handle skins and all that.
            var temp1 = new GameObject("replace prefab holder");
            temp1.SetActive(false);
            var temp2 = Instantiate(comp.ReplacePrefab, temp1.transform);
            ComponentProcessor.ProcessComponents(temp2, holder);

            return new ExplosionModelHandler.GameObjectSwapData(comp.gameObject, temp2);
        }

        private static ExplosionModelHandler.MaterialSwapData ToData(SwapMaterialOnExplosion comp, MaterialHolder holder)
        {
            return new ExplosionModelHandler.MaterialSwapData
            {
                swapMaterial = comp.Material ?
                    (comp.GenerateExplodedMaterialProcedurally ? MaterialHolder.GenerateProceduralExplosionMaterial(comp.Material) : comp.Material) :
                    holder.GetMaterial(comp.DefaultMaterial, comp.MaterialObjectPath, comp.FromInterior, comp.GenerateExplodedMaterialProcedurally),
                affectedGameObjects = comp.AffectedGameObjects
            };
        }
    }
}
