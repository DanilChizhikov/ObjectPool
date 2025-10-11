using NUnit.Framework;
using UnityEngine;

namespace DTech.ObjectPool.Tests
{
	[TestFixture]
	internal sealed class PoolServiceTests
	{
		private PoolService _poolService;
		private GameObject _origin;
		private TestComponent _componentOrigin;

		[SetUp]
		public void Setup()
		{
			_origin = new GameObject("Test GameObject");
			_componentOrigin = new GameObject("Test Component").AddComponent<TestComponent>();
			_origin.hideFlags = HideFlags.DontSave;
			_poolService = new PoolService();
		}
		
		[Test]
		[TestCase(10), TestCase(5), TestCase(100)]
		public void PrepareClones_GameObject_WithInitialCapacity_ThenCloneCountEqualsInitialCapacity(int initialCapacity)
		{
			IPoolConfig config = new TestPoolConfig
			{
				MaxCapacity = int.MaxValue,
				InitialCapacity = initialCapacity,
			};
			
			_poolService.PrepareClones(_origin, config);
			
			Assert.AreEqual(initialCapacity, _poolService.GetCloneCount(_origin, CloneScope.All));
		}
		
		[Test]
		[TestCase(10), TestCase(5), TestCase(100)]
		public void PrepareClones_Component_WithInitialCapacity_ThenCloneCountEqualsInitialCapacity(int initialCapacity)
		{
			IPoolConfig config = new TestPoolConfig
			{
				MaxCapacity = int.MaxValue,
				InitialCapacity = initialCapacity,
			};
			
			_poolService.PrepareClones(_componentOrigin, config);
			
			Assert.AreEqual(initialCapacity, _poolService.GetCloneCount(_componentOrigin, CloneScope.All));
		}
		
		[Test]
		public void GetClone_GameObject_ThenCloneCountEqualsInitialCapacity()
		{
			_poolService.PrepareClones(_origin, new TestPoolConfig
			{
				MaxCapacity = int.MaxValue,
				InitialCapacity = 1,
			});
			
			GameObject clone = _poolService.GetClone(_origin);
			Assert.AreEqual(1, _poolService.GetCloneCount(_origin, CloneScope.Active));
			Assert.AreEqual(0, _poolService.GetCloneCount(_origin, CloneScope.Passive));
			Assert.AreEqual(1, _poolService.GetCloneCount(_origin, CloneScope.All));
		}
		
		[Test]
		public void GetClone_Component_ThenCloneCountEqualsInitialCapacity()
		{
			_poolService.PrepareClones(_componentOrigin, new TestPoolConfig
			{
				MaxCapacity = int.MaxValue,
				InitialCapacity = 1,
			});
			
			TestComponent clone = _poolService.GetClone(_componentOrigin);
			Assert.AreEqual(1, _poolService.GetCloneCount(_componentOrigin, CloneScope.Active));
			Assert.AreEqual(0, _poolService.GetCloneCount(_componentOrigin, CloneScope.Passive));
			Assert.AreEqual(1, _poolService.GetCloneCount(_componentOrigin, CloneScope.All));
		}
		
		[Test]
		public void ReturnClone_GameObject_ThenCloneCountEqualsInitialCapacity()
		{
			_poolService.PrepareClones(_origin, new TestPoolConfig
			{
				MaxCapacity = int.MaxValue,
				InitialCapacity = 1,
			});
			
			GameObject clone = _poolService.GetClone(_origin);
			_poolService.ReturnClone(clone);
			Assert.AreEqual(0, _poolService.GetCloneCount(_origin, CloneScope.Active));
			Assert.AreEqual(1, _poolService.GetCloneCount(_origin, CloneScope.Passive));
			Assert.AreEqual(1, _poolService.GetCloneCount(_origin, CloneScope.All));
		}
		
		[Test]
		public void ReturnClone_Component_ThenCloneCountEqualsInitialCapacity()
		{
			_poolService.PrepareClones(_componentOrigin, new TestPoolConfig
			{
				MaxCapacity = int.MaxValue,
				InitialCapacity = 1,
			});
			
			TestComponent clone = _poolService.GetClone(_componentOrigin);
			_poolService.ReturnClone(clone);
			Assert.AreEqual(0, _poolService.GetCloneCount(_componentOrigin, CloneScope.Active));
			Assert.AreEqual(1, _poolService.GetCloneCount(_componentOrigin, CloneScope.Passive));
			Assert.AreEqual(1, _poolService.GetCloneCount(_componentOrigin, CloneScope.All));
		}

		[Test]
		public void Reset_Component_ThenCloneWasResetTrue()
		{
			_poolService.PrepareClones(_componentOrigin, new TestPoolConfig
			{
				MaxCapacity = int.MaxValue,
				InitialCapacity = 1,
			});
			
			TestComponent clone = _poolService.GetClone(_componentOrigin);
			clone.WasReset = false;
			_poolService.ReturnClone(clone);
			clone = _poolService.GetClone(_componentOrigin);
			
			Assert.IsTrue(clone.WasReset);
		}
		
		[TearDown]
		public void TearDown()
		{
			_poolService.Dispose();
			Object.Destroy(_origin);
		}
	}
}