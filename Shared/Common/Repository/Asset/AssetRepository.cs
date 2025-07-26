using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.Common.Repository
{
    public class AssetRepository<TID, T> : IAssetRepository<TID, T>
        where TID : notnull where T : notnull
    {
        private readonly Dictionary<TID, T> _repository = new();
        private readonly IRepositoryAsserter _repositoryAsserter;

        public AssetRepository()
        {
            _repositoryAsserter = new RepositoryAsserter(new AssertFound(new ThrowHandler()), new AssertNotNull(new ThrowHandler()),
                new AssertNonDuplicate(new ThrowHandler()));
        }

        public AssetRepository(IRepositoryAsserter repositoryAsserter)
        {
            _repositoryAsserter = repositoryAsserter;
        }

        public void Add(TID key, T value)
        {
            _repositoryAsserter.AssertUnique(value, () => _repository.ContainsKey(key));

            _repository.Add(key, value);
        }

        public void Remove(TID key)
        {
            _repositoryAsserter.AssertFound(key, () => _repository.ContainsKey(key));

            _repository.Remove(key);
        }

        public T Get(TID key)
        {
            _repositoryAsserter.AssertFound(key, () => _repository.ContainsKey(key));

            return _repository[key];
        }

        public bool Contains(TID key)
        {
            return _repository.ContainsKey(key);
        }
    }
}