
namespace DTech.ObjectPool
{
    public interface IPoolService
    {
        IPoolProvider<TConfig, TObject> GetProvider<TConfig, TObject>(TConfig config) where TConfig : IPoolConfig where TObject : class;
        void Clear(IPoolConfig config);
    }
}
