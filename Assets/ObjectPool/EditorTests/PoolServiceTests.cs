using System;
using NUnit.Framework;

namespace DTech.ObjectPool.EditorTests
{
	[TestFixture]
	public sealed class PoolServiceTests
	{
		private PoolService _poolService;

		[SetUp]
		public void Setup()
		{
			IPoolProvider provider = new TestPoolProvider();
			_poolService = new PoolService(new[]
			{
				provider
			});
		}
		
		[Test]
		public void GetPoolProvider()
		{
			var config = new TestPoolConfig
			{
				PoolName = new Guid().ToString(),
				DefaultCapacity = 10,
				MaxCapacity = 10,
			};

			Assert.DoesNotThrow(() => _poolService.GetProvider<TestPoolConfig, TestObject>(config));
		}

		[Test]
		public void PoolProviderPrepareClone()
		{
			var config = new TestPoolConfig
			{
				PoolName = new Guid().ToString(),
				DefaultCapacity = 10,
				MaxCapacity = 10,
			};
			IPoolProvider<TestPoolConfig,TestObject> provider = _poolService.GetProvider<TestPoolConfig, TestObject>(config);
			Assert.DoesNotThrow(() => provider.PrepareClones(config, config.MaxCapacity));
		}

		[TearDown]
		public void TearDown()
		{
			_poolService?.Dispose();
		}
	}
}