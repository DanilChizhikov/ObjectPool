using MbsCore.ObjectPool.Infrastructure;
using MbsCore.ObjectPool.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace MbsCore.ObjectPool.Tests
{
    internal sealed class ObjectPoolTest
    {
        private const int PrepareCount = 20;
        
        private IPoolService _poolService;
        private IPoolContext _poolContext;

        [SetUp]
        public void Setup()
        {
            _poolContext = new GameObject(nameof(PoolContext)).AddComponent<PoolContext>();
            _poolService = new PoolService(_poolContext);
        }

        [Test]
        public void PrepareTest()
        {
            GameObject exampleGameObject = new GameObject("Example");
            _poolService.PrepareClones(exampleGameObject, PrepareCount);
            bool cloneAreEqualPrepareCount = _poolService.GetCloneCount(exampleGameObject, CloneScope.All) == PrepareCount;
            _poolService.Remove(exampleGameObject);
            Object.Destroy(exampleGameObject);
            Assert.AreEqual(true, cloneAreEqualPrepareCount);
        }
    }
}
