using System;
using System.Collections.Generic;
using MbsCore.ObjectPool.Infrastructure;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace MbsCore.ObjectPool.Runtime
{
    internal sealed class GameObjectPool : IDisposable
    {
        public event Action<GameObject> OnClonePreDestroy; 
        
        private readonly GameObject _origin;
        private readonly Transform _parent;
        private readonly Dictionary<GameObject, HashSet<IResettable>> _resettableMap;
        private readonly ObjectPool<GameObject> _objectPool;

        public GameObjectPool(GameObject origin, Transform parent, int capacity, int maxSize)
        {
            _origin = origin;
            _parent = parent;
            _resettableMap = new Dictionary<GameObject, HashSet<IResettable>>();
            _objectPool = new ObjectPool<GameObject>(CreateClone, PrepareClone, ReleaseClone, DestroyClone,
                                                     defaultCapacity: capacity, maxSize: maxSize);
        }

        public GameObject GetClone() => _objectPool.Get();

        public void Return(GameObject clone) => _objectPool.Release(clone);
        
        public void Dispose()
        {
            _objectPool.Dispose();
        }

        private GameObject CreateClone()
        {
            GameObject clone = Object.Instantiate(_origin, _parent);
            IResettable[] resettables = clone.GetComponentsInChildren<IResettable>();
            _resettableMap.Add(clone, new HashSet<IResettable>(resettables));
            clone.SetActive(false);
            return clone;
        }
        
        private void PrepareClone(GameObject clone)
        {
            if (_resettableMap.TryGetValue(clone, out HashSet<IResettable> resettables))
            {
                foreach (var resettable in resettables)
                {
                    resettable.ResetSettings();
                }
            }
            
            clone.transform.SetParent(null);
            clone.SetActive(true);
        }
        
        private void ReleaseClone(GameObject clone)
        {
            clone.transform.SetParent(_parent);
            clone.SetActive(false);
        }
        
        private void DestroyClone(GameObject clone)
        {
            OnClonePreDestroy?.Invoke(clone);
            _resettableMap.Remove(clone);
            Object.Destroy(clone);
        }
    }
}