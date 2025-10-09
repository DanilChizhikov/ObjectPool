using System;
using System.Collections.Generic;

namespace DTech.ObjectPool
{
	public sealed class PoolService : IPoolService, IDisposable
	{
		private readonly List<IPoolProvider> _providers;
		private readonly Dictionary<Type, int> _providerIndexMap;

		private bool _isDisposed;

		public PoolService(IEnumerable<IPoolProvider> providers)
		{
			_providers = new List<IPoolProvider>(providers);
			_providerIndexMap = new Dictionary<Type, int>();
			for (int i = 0; i < _providers.Count; i++)
			{
				IPoolProvider provider = _providers[i];
				_providerIndexMap.Add(provider.ServicedConfigType, i);
			}
		}
		
		public IPoolProvider<TConfig, TObject> GetProvider<TConfig, TObject>(TConfig config)
			where TConfig : IPoolConfig where TObject : class
		{
			ThrowIfDisposed();
			IPoolProvider provider = GetProviderOrThrow(config.GetType());
			if (provider is not IPoolProvider<TConfig, TObject> genericProvider)
			{
				throw new Exception($"Provider not found [{config.GetType().Name}]");
			}

			return genericProvider;
		}

		public void Clear(IPoolConfig config)
		{
			ThrowIfDisposed();
			GetProviderOrThrow(config.GetType()).Clear();
		}

		public void Dispose()
		{
			if (!_isDisposed)
			{
				return;
			}
			
			for (int i = 0; i < _providers.Count; i++)
			{
				_providers[i].Dispose();
			}
			
			_isDisposed = true;
		}

		private void ThrowIfDisposed()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		private IPoolProvider GetProviderOrThrow(Type configType)
		{
			bool hasProvider = _providerIndexMap.TryGetValue(configType, out int index);
			if (!hasProvider)
			{
				index = GetProviderIndex(configType);
				hasProvider = index >= 0;
				if (hasProvider)
				{
					_providerIndexMap[configType] = index;
				}
			}

			if (!hasProvider)
			{
				throw new Exception($"Provider not found [{configType.Name}]");
			}

			return _providers[index];
		}

		private int GetProviderIndex(Type configType)
		{
			int smallestWeight = int.MaxValue;
			int index = -1;
			for (int i = 0; i < _providers.Count; i++)
			{
				IPoolProvider provider = _providers[i];
				if (!provider.ServicedConfigType.IsAssignableFrom(configType))
				{
					continue;
				}

				int weight = provider.ServicedConfigType.Comparison(configType);
				if (weight <= smallestWeight)
				{
					smallestWeight = weight;
					index = i;
				}
			}
			
			return index;
		}
	}
}