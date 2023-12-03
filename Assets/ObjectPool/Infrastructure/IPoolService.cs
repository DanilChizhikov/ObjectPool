using UnityEngine;

namespace MbsCore.ObjectPool.Infrastructure
{
    public interface IPoolService
    {
        void PrepareClones<T>(T origin, int capacity) where T : Component;
        void PrepareClones(GameObject origin, int capacity);
        T GetClone<T>(T origin, Transform parent = null) where T : Component;
        T GetClone<T>(T origin, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component;
        GameObject GetClone(GameObject origin, Transform parent = null);
        GameObject GetClone(GameObject origin, Vector3 position, Quaternion rotation, Transform parent = null);
        void ReturnClone<T>(T clone) where T : Component;
        void ReturnClone(GameObject clone);
        void Remove<T>(T origin) where T : Component;
        void Remove(GameObject origin);
    }
}
