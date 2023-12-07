using System;
using System.Collections.Generic;
using MbsCore.ObjectPool.Infrastructure;
using UnityEngine;
using Object = System.Object;

namespace MbsCore.ObjectPool.Runtime
{
    [AddComponentMenu("MbsCore/Pool/Context")]
    public sealed class PoolContext : MonoBehaviour, IPoolContext
    {
        private readonly Dictionary<GameObject, GameObjectPool> _poolMap = new();
        private readonly Dictionary<GameObject, GameObject> _originMap = new();
        private readonly Dictionary<GameObject, Dictionary<Type, Object>> _entityMap = new();

        [SerializeField, Min(1)] private int _poolMaxSize = 10000;

        public int GetCloneCount(GameObject origin, CloneScope scope)
        {
            if (!_poolMap.TryGetValue(origin, out GameObjectPool pool))
            {
                return 0;
            }

            if (!pool.ScopeMap.TryGetValue(scope, out int count))
            {
                count = 0;
            }

            return count;
        }

        public T GetClone<T>(T origin, int capacity) where T : Component
        {
            GameObject clone = GetClone(origin.gameObject, capacity);
            if (!_entityMap.TryGetValue(clone, out Dictionary<Type, Object> entities))
            {
                entities = new Dictionary<Type, object>();
                _entityMap.Add(clone, entities);
            }

            Type entityType = typeof(T);
            if (!entities.TryGetValue(entityType, out Object entity) &&
                clone.TryGetComponent(out T entityEssence))
            {
                entity = entityEssence;
                entities.Add(entityType, entity);
            }

            return (T)entity;
        }

        public GameObject GetClone(GameObject origin, int capacity)
        {
            if (!_poolMap.TryGetValue(origin, out GameObjectPool pool))
            {
                pool = new GameObjectPool(origin, transform, capacity, _poolMaxSize);
                pool.OnClonePreDestroy += ClonePreDestroyHandler;
                _poolMap.Add(origin, pool);
            }

            GameObject clone = pool.GetClone();
            _originMap.Add(clone, origin);
            return clone;
        }

        public void ReturnClone(GameObject clone)
        {
            if (!_originMap.TryGetValue(clone, out GameObject origin) ||
                !_poolMap.TryGetValue(origin, out GameObjectPool pool))
            {
                return;
            }

            _originMap.Remove(clone);
            pool.Return(clone);
        }

        public void Remove(GameObject origin)
        {
            if (!_poolMap.TryGetValue(origin, out GameObjectPool pool))
            {
                return;
            }

            _poolMap.Remove(origin);
            pool.Dispose();
            pool.OnClonePreDestroy -= ClonePreDestroyHandler;
        }

        public void Clear()
        {
            var origins = new HashSet<GameObject>(_poolMap.Keys);
            foreach (var origin in origins)
            {
                Remove(origin);
            }
        }
        
        private void ClonePreDestroyHandler(GameObject clone)
        {
            _originMap.Remove(clone);
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}