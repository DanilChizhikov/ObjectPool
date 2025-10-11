using System;
using System.Collections.Generic;
using DTech.Extensions.Runtime;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace DTech.ObjectPool
{
    public sealed class PoolService : IPoolService, IDisposable
    {
        private const int DefaultCapacity = 1;
        
        private readonly Dictionary<GameObject, PoolContent> _poolMap = new();
        private readonly Dictionary<GameObject, GameObject> _originMap = new();

        public int GetCloneCount<T>(T origin, CloneScope scope)
            where T : Component
        {
            return GetCloneCount(origin.gameObject, scope);
        }

        public int GetCloneCount(GameObject origin, CloneScope scope)
        {
            PoolContent context = GetPoolContext(origin, 1, int.MaxValue);
            return context.GetCloneCount(scope);
        }

        public void PrepareClones<T>(T origin, IPoolConfig config)
            where T : Component
        {
            PrepareClones(origin.gameObject, config);
        }

        public void PrepareClones(GameObject origin, IPoolConfig config)
        {
            PoolContent context = GetPoolContext(origin.gameObject, config.InitialCapacity, config.MaxCapacity);
            PrepareClones(context, config.InitialCapacity);
        }

        public T GetClone<T>(T origin, Transform parent = null) where T : Component
        {
            return GetClone(origin, Vector3.zero, Quaternion.identity, parent);
        }

        public T GetClone<T>(T origin, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            return GetClone(origin.gameObject, position, rotation, parent)
                .GetComponent<T>()
                .ThrowIfNull();
        }

        public GameObject GetClone(GameObject origin, Transform parent = null)
        {
            return GetClone(origin, Vector3.zero, Quaternion.identity, parent);
        }

        public GameObject GetClone(GameObject origin, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            PoolContent context = GetPoolContext(origin.gameObject, DefaultCapacity, int.MaxValue);
            GameObject clone = context.GetClone();
            _originMap.Add(clone, origin);
            PrepareClone(ref clone, position, rotation, parent);
            return clone;
        }

        public void ReturnClone<T>(T clone) where T : Component
        {
            ReturnClone(clone.gameObject);
        }

        public void ReturnClone(GameObject clone)
        {
            if (!_originMap.Remove(clone, out GameObject origin))
            {
                return;
            }
            
            PoolContent context = GetPoolContext(origin, DefaultCapacity, int.MaxValue);
            context.ReturnClone(clone);
        }

        public void Remove<T>(T origin) where T : Component
        {
            Remove(origin.gameObject);
        }

        public void Remove(GameObject origin)
        {
            if (!_poolMap.Remove(origin, out PoolContent content))
            {
                return;
            }
            
            Object.Destroy(content);
        }
        
        public void Dispose()
        {
            Dictionary<GameObject, PoolContent>.ValueCollection contents = _poolMap.Values;
            foreach (PoolContent content in contents)
            {
                Object.Destroy(content);
            }
            
            _poolMap.Clear();
            _originMap.Clear();
        }
        
        private PoolContent GetPoolContext(GameObject origin, int capacity, int maxSize)
        {
            if (!_poolMap.TryGetValue(origin, out PoolContent poolContext))
            {
                poolContext = new GameObject().AddComponent<GameObjectPoolContent>();
                poolContext.Initialize(origin, capacity, maxSize);
                _poolMap.Add(origin, poolContext);
            }
            
            return poolContext;
        }
        
        private void PrepareClones(PoolContent context, int initialCount)
        {
            List<GameObject> clones = ListPool<GameObject>.Get();
            for (int i = 0; i < initialCount; i++)
            {
                GameObject clone = context.GetClone();
                clones.Add(clone);
            }

            while (clones.Count > 0)
            {
                GameObject clone = clones[0];
                clones.RemoveAt(0);
                context.ReturnClone(clone);
            }
            
            ListPool<GameObject>.Release(clones);
        }

        private void PrepareClone(ref GameObject clone, Vector3 position, Quaternion rotation, Transform parent)
        {
            Transform cloneTransform = clone.transform;
            cloneTransform.SetParent(parent);
            cloneTransform.position = position;
            cloneTransform.rotation = rotation;
            cloneTransform.localScale = Vector3.one;
        }
    }
}