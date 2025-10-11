namespace DTech.ObjectPool.Tests
{
	internal sealed class TestPoolConfig : IPoolConfig
	{
		public int MaxCapacity { get; set; }
		public int InitialCapacity { get; set; }
	}
}