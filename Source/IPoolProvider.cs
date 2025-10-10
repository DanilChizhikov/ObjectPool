using System;

namespace DTech.ObjectPool
{
	public interface IPoolProvider : IDisposable
	{
		Type ServicedConfigType { get; }
		
		void Clear();
	}

	public interface IPoolProvider<in TConfig, TObject> : IPoolProvider
		where TConfig : IPoolConfig
		where TObject : class
	{
		void PrepareClones(TConfig config, int count);
		TObject GetClone(TConfig config);
		void ReleaseClone(TObject clone);
	}
}