namespace DTech.ObjectPool
{
	public interface IPoolConfig
	{
		int MaxCapacity { get; }
		int InitialCapacity { get; }
	}
}