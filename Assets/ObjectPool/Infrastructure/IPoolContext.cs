using UnityEngine;

namespace MbsCore.ObjectPool.Infrastructure
{
    public interface IPoolContext
    {
        T GetClone<T>(T origin, int capacity) where T : Component;
        GameObject GetClone(GameObject origin, int capacity);
        void ReturnClone(GameObject clone);
        void Remove(GameObject origin);
        void Clear();
    }
}