namespace IdelPog.Infrastructure.Repository
{
    public class AssetRepository<TID, T> : IAssetRepository<TID, T> where TID : notnull
    {
        private readonly Dictionary<TID, T> _repository = new();
        
        public void Add(TID key, T entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(TID key)
        {
            throw new NotImplementedException();
        }

        public T Get(TID key)
        {
            throw new NotImplementedException();
        }

        public bool Contains(TID key)
        {
            throw new NotImplementedException();
        }
    }
}