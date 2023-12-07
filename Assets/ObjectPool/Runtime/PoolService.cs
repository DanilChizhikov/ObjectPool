using MbsCore.ObjectPool.Infrastructure;
using UnityEngine;

namespace MbsCore.ObjectPool.Runtime
{
    public sealed class PoolService : IPoolService
    {
        private const int DefaultCapacity = 10;
        
        private readonly IPoolContext _poolContext;

        public PoolService(IPoolContext poolContext)
        {
            _poolContext = poolContext;
        }

        public int GetCloneCount<T>(T origin, CloneScope scope) where T : Component =>
                GetCloneCount(origin.gameObject, scope);

        public int GetCloneCount(GameObject origin, CloneScope scope) => _poolContext.GetCloneCount(origin, scope);

        public void PrepareClones<T>(T origin, int capacity) where T : Component
        {
            PrepareClones(origin.gameObject, capacity);
        }

        public void PrepareClones(GameObject origin, int capacity)
        {
            GameObject clone = _poolContext.GetClone(origin, capacity);
            _poolContext.ReturnClone(clone);
        }

        public T GetClone<T>(T origin, Transform parent = null) where T : Component
        {
            return GetClone(origin, Vector3.zero, Quaternion.identity, parent);
        }

        public T GetClone<T>(T origin, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            PrepareClones(origin, DefaultCapacity);
            T clone = _poolContext.GetClone(origin, DefaultCapacity);
            GameObject cloneGameObject = clone.gameObject;
            PrepareClone(ref cloneGameObject, position, rotation, parent);
            return clone;
        }

        public GameObject GetClone(GameObject origin, Transform parent = null)
        {
            return GetClone(origin, Vector3.zero, Quaternion.identity, parent);
        }

        public GameObject GetClone(GameObject origin, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            PrepareClones(origin, DefaultCapacity);
            GameObject clone = _poolContext.GetClone(origin, DefaultCapacity);
            PrepareClone(ref clone, position, rotation, parent);
            return clone;
        }

        public void ReturnClone<T>(T clone) where T : Component
        {
            ReturnClone(clone.gameObject);
        }

        public void ReturnClone(GameObject clone)
        {
            _poolContext.ReturnClone(clone);
        }

        public void Remove<T>(T origin) where T : Component
        {
            Remove(origin.gameObject);
        }

        public void Remove(GameObject origin)
        {
            _poolContext.Remove(origin);
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