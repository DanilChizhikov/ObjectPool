using NUnit.Framework;
using UnityEngine;

namespace MbsCore.ObjectPool.Tests
{
    internal sealed class ObjectPoolTest
    {
        private class TestObjectPoolSettings : IObjectPoolSettings
        {
            public int Capacity => 1000;
        }
        
        private const int PrepareCount = 20;

        [Test]
        public void Prepare_20_Clones_For_Example_GameObject_Test()
        {
            IPoolService poolService = new PoolService(new TestObjectPoolSettings());
            GameObject exampleGameObject = new GameObject("Example");
            poolService.PrepareClones(exampleGameObject, PrepareCount);
            bool cloneAreEqualPrepareCount = poolService.GetCloneCount(exampleGameObject, CloneScope.All) == PrepareCount;
            poolService.Remove(exampleGameObject);
            Object.Destroy(exampleGameObject);
            Assert.AreEqual(true, cloneAreEqualPrepareCount);
        }
    }
}
