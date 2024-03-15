using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace MbsCore.ObjectPool
{
    internal sealed class PoolContext : IDisposable
    {
        private const int MinCapacity = 10;

        private readonly int _capacity;
        private readonly Transform _container;
        private readonly Dictionary<GameObject, GameObjectPool> _poolMap;
        private readonly Dictionary<GameObject, GameObject> _originMap;
        private readonly Dictionary<GameObject, Dictionary<Type, Object>> _entityMap;

        private bool _isDisposed;
        
        public PoolContext(int capacity)
        {
            _capacity = Math.Max(capacity, MinCapacity);
            _container = new GameObject(nameof(PoolContext)).transform;
            UnityEngine.Object.DontDestroyOnLoad(_container);
            _poolMap = new Dictionary<GameObject, GameObjectPool>();
            _originMap = new Dictionary<GameObject, GameObject>();
            _entityMap = new Dictionary<GameObject, Dictionary<Type, Object>>();
            _isDisposed = false;
        }
        
        public int GetCloneCount(GameObject origin, CloneScope scope)
        {
            if (!_poolMap.TryGetValue(origin, out GameObjectPool pool))
            {
                return 0;
            }

            return pool.ScopeMap.GetValueOrDefault(scope, 0);
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
                pool = new GameObjectPool(origin, CreateContainer(origin.name), capacity, _capacity);
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
            if (!_poolMap.Remove(origin, out GameObjectPool pool))
            {
                return;
            }

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
        
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            
            Clear();
            UnityEngine.Object.Destroy(_container.gameObject);
            _isDisposed = true;
        }
        
        private void ClonePreDestroyHandler(GameObject clone)
        {
            _originMap.Remove(clone);
        }

        private Transform CreateContainer(string originName)
        {
            Transform container = new GameObject(originName).transform;
            container.SetParent(_container);
            return container;
        }
    }
}