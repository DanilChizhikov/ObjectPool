namespace DTech.ObjectPool.EditorTests
{
	internal sealed class TestPoolConfig : IPoolConfig
	{
		public string PoolName { get; set; }
		public int DefaultCapacity { get; set; }
		public int MaxCapacity { get; set; }
	}
}