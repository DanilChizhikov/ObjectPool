namespace DTech.ObjectPool
{
	public interface IPoolConfig
	{
		string PoolName { get; }
		int DefaultCapacity { get; }
		int MaxCapacity { get; }
	}
}