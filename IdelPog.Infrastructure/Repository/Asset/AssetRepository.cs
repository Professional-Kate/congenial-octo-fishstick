namespace IdelPog.Infrastructure.Repository
{
    public class AssetRepository<TID, T>(IRepositoryAsserter repositoryAsserter) : IAssetRepository<TID, T> 
        where TID : notnull
    {
        private readonly Dictionary<TID, T> _repository = new();
        
        public void Add(TID key, T value)
        {
            repositoryAsserter.AssertUnique(value!, () => _repository.ContainsKey(key));
            
            _repository.Add(key, value);
        }

        public void Remove(TID key)
        {
            repositoryAsserter.AssertFound(key, () => _repository.ContainsKey(key));

            _repository.Remove(key);
        }

        public T Get(TID key)
        {
            repositoryAsserter.AssertFound(key, () => _repository.ContainsKey(key));

            return _repository[key];
        }

        public bool Contains(TID key)
        {
            return _repository.ContainsKey(key);
        }
    }
}