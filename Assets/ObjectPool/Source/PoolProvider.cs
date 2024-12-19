using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace DTech.ObjectPool
{
	public abstract class PoolProvider<TConfig, TObject> : IPoolProvider<TConfig, TObject>
		where TConfig : IPoolConfig
		where TObject : class
	{
		private readonly Dictionary<string, ObjectPool<TObject>> _poolMap = new();
		private readonly Dictionary<TObject, ObjectPool<TObject>> _poolByCloneMap = new();
		private readonly List<ObjectPool<TObject>> _pools = new();
		
		public Type ServicedConfigType => typeof(TConfig);

		public void PrepareClones(TConfig config, int count)
		{
			ObjectPool<TObject> pool = GetPool(config);
			var clones = new TObject[count];
			for (int i = 0; i < count; i++)
			{
				TObject clone = pool.Get();
				clones[i] = clone;
			}
			
			for (int i = 0; i < clones.Length; i++)
			{
				pool.Release(clones[i]);
			}
		}

		public TObject GetClone(TConfig config)
		{
			ObjectPool<TObject> pool = GetPool(config);
			TObject clone = pool.Get();
			_poolByCloneMap.Add(clone, pool);
			return clone;
		}

		public void ReleaseClone(TObject clone)
		{
			if (_poolByCloneMap.Remove(clone, out ObjectPool<TObject> pool))
			{
				pool.Release(clone);
			}
		}

		public void Clear()
		{
			for (int i = 0; i < _pools.Count; i++)
			{
				_pools[i].Clear();
			}
		}
		
		public void Dispose()
		{
			for (int i = 0; i < _pools.Count; i++)
			{
				_pools[i].Dispose();
			}
		}

		protected abstract ObjectPool<TObject> CreatePool(TConfig config);
		
		private ObjectPool<TObject> GetPool(TConfig config)
		{
			if (!_poolMap.TryGetValue(config.PoolName, out ObjectPool<TObject> pool))
			{
				pool = CreatePool(config);
				_poolMap[config.PoolName] = pool;
				_pools.Add(pool);
			}

			return pool;
		}
	}
}