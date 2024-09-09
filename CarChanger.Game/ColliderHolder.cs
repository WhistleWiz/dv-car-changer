using System.Reflection;
using UnityEngine;

namespace CarChanger.Game
{
    internal class ColliderHolder
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

        private const int CollisionLayer = 10;
        private const int WalkableLayer = 11;
        private const int ItemLayer = 13;

        private TrainCarColliders _colliders;
        private GameObject? _collision;
        private GameObject? _walkable;
        private GameObject? _items;
        private GameObject _collisionInstance = null!;
        private GameObject _walkableInstance = null!;
        private GameObject _itemsInstance = null!;

        public ColliderHolder(TrainCar car, GameObject? collision, GameObject? walkable, GameObject? items)
        {
            _colliders = car.GetComponent<TrainCarColliders>();
            _collision = collision;
            _walkable = walkable;
            _items = items;
        }

        public void Apply()
        {
            var roots = GetRoots();

            if (_collision != null)
            {
                _collisionInstance = Object.Instantiate(_collision, roots.Collision);
                Helpers.SetLayersForAllChildren(_collisionInstance, CollisionLayer);

                if (_collisionInstance.TryGetComponentInParent<TrainCar>(out var root))
                {
                    ComponentProcessor.ProcessHideTransforms(_collisionInstance, root.transform);
                }
            }

            if (_walkable != null)
            {
                _walkableInstance = Object.Instantiate(_walkable, roots.Walkable);
                Helpers.SetLayersForAllChildren(_walkableInstance, WalkableLayer);
                ComponentProcessor.ProcessTeleportPassThroughColliders(_walkableInstance);

                if (_collisionInstance.TryGetComponentInParent<TrainCarInteriorObject>(out var root))
                {
                    ComponentProcessor.ProcessHideTransforms(_walkableInstance, root.transform);
                }
            }

            if (_items != null)
            {
                _itemsInstance = Object.Instantiate(_items, roots.Items);
                Helpers.SetLayersForAllChildren(_itemsInstance, ItemLayer);

                if (_collisionInstance.TryGetComponentInParent<TrainCarInteriorObject>(out var root))
                {
                    ComponentProcessor.ProcessHideTransforms(_itemsInstance, root.transform);
                }
            }
        }

        public void Unapply()
        {
            Helpers.DestroyIfNotNull(_collisionInstance);
            Helpers.DestroyIfNotNull(_walkableInstance);
            Helpers.DestroyIfNotNull(_itemsInstance);
        }

        private (Transform Collision, Transform Walkable, Transform Items) GetRoots()
        {
            var t = typeof(TrainCarColliders);
            var c = t.GetField("collisionRoot", Flags).GetValue(_colliders);
            var w = t.GetField("walkableRoot", Flags).GetValue(_colliders);
            var i = t.GetField("itemsEnvironmentRoot", Flags).GetValue(_colliders);
            return ((Transform)c, (Transform)w, (Transform)i);
        }
    }
}
