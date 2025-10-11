using System.Collections.Generic;
using UnityEngine;

namespace DTech.ObjectPool
{
	internal abstract class PoolContent : MonoBehaviour
	{
		private const int MinCapacity = 1;

		private bool _isInitialized = false;
		private GameObjectPool _pool;
		private bool _isDisposed = false;
		
		public virtual void Initialize(GameObject origin, int capacity, int maxSize)
		{
			capacity = Mathf.Max(capacity, MinCapacity);
			_pool = new GameObjectPool(origin, transform, capacity, maxSize);
			name = $"{origin.name}_PoolContent";
			_pool.OnClonePreDestroy += ClonePreDestroyHandler;
			_isInitialized = true;
		}

		public int GetCloneCount(CloneScope scope)
		{
			ThrowIfNotInitialized(nameof(GetCloneCount));
			ThrowIfDisposed(nameof(GetCloneCount));
			return _pool.ScopeMap.GetValueOrDefault(scope, 0);
		}

		public GameObject GetClone()
		{
			ThrowIfNotInitialized(nameof(GetClone));
			ThrowIfDisposed(nameof(GetClone));
			return _pool.GetClone();
		}

		public void ReturnClone(GameObject clone)
		{
			ThrowIfNotInitialized(nameof(ReturnClone));
			ThrowIfDisposed(nameof(ReturnClone));
			_pool.Return(clone);
		}
		
		public void Clear()
		{
			ThrowIfNotInitialized(nameof(Clear));
			ThrowIfDisposed(nameof(Clear));
			_pool.Clear();
		}
		
		protected virtual void Awake()
		{
			gameObject.hideFlags = HideFlags.DontSave;
			DontDestroyOnLoad(gameObject);
		}

		protected virtual void OnDestroy()
		{
			if (!_isInitialized || _isDisposed)
			{
				return;
			}
			
			Clear();
			_pool.OnClonePreDestroy -= ClonePreDestroyHandler;
			_pool.Dispose();
			_isDisposed = true;
		}
		
		protected virtual void ClonePreDestroyHandler(GameObject clone)
		{
		}

		private void ThrowIfNotInitialized(string placement)
		{
			if (!_isInitialized)
			{
				throw new PoolContentNotInitializedException(this, placement);
			}
		}
		
		private void ThrowIfDisposed(string placement)
		{
			if (_isDisposed)
			{
				throw new PoolContentDisposedException(this, placement);
			}
		}
	}
}