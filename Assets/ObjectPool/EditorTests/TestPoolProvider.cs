using UnityEngine.Pool;

namespace DTech.ObjectPool.EditorTests
{
	internal sealed class TestPoolProvider : PoolProvider<TestPoolConfig, TestObject>
	{
		protected override ObjectPool<TestObject> CreatePool(TestPoolConfig config) =>
			new (Create, Get, Release, Destroy, defaultCapacity: config.DefaultCapacity, maxSize: config.MaxCapacity);

		private TestObject Create() =>
			new TestObject
			{
				IsActive = false,
				IsDestroyed = false,
			};
		
		private void Get(TestObject clone) => clone.IsActive = true;
		private void Release(TestObject clone) => clone.IsActive = false;
		private void Destroy(TestObject clone) => clone.IsDestroyed = true;
	}
}