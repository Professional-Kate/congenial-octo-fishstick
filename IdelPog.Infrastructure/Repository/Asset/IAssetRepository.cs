namespace IdelPog.Infrastructure.Repository
{
    public interface IAssetRepository<in TID, T>
    {
        public void Add(TID key, T value);
        
        public void Remove(TID key);
        
        public T Get(TID key);
        
        public bool Contains(TID key);
    }
}