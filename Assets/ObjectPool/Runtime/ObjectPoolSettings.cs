using UnityEngine;

namespace MbsCore.ObjectPool
{
    [CreateAssetMenu(menuName = "MbsCore/ObjectPool/" + nameof(ObjectPoolSettings), fileName = nameof(ObjectPoolSettings))]
    public class ObjectPoolSettings : ScriptableObject, IObjectPoolSettings
    {
        [SerializeField] private int _capacity = 1000;

        public int Capacity => _capacity;
    }
}