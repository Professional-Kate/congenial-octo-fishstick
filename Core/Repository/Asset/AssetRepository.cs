using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Handler;

namespace IdelPog.Core.Repository.Asset
{
    public class AssetRepository<TID, T> : IAssetRepository<TID, T>
        where TID : notnull where T : notnull
    {
        private readonly Dictionary<TID, T> _repository = new();
        private readonly IRepositoryAsserter _repositoryAsserter;

        public AssetRepository()
        {
            _repositoryAsserter = new RepositoryAsserter(new FoundAssertion(new ThrowHandler()), new ObjectNullAssertion(new ThrowHandler()),
                new UniqueAssertion(new ThrowHandler()));
        }

        public AssetRepository(IRepositoryAsserter repositoryAsserter)
        {
            _repositoryAsserter = repositoryAsserter;
        }

        public void Add(TID key, T value)
        {
            _repositoryAsserter.AssertUnique(value, _repository.ContainsKey(key));

            _repository.Add(key, value);
        }

        public void Remove(TID key)
        {
            _repositoryAsserter.AssertFound(key, _repository.ContainsKey(key));

            _repository.Remove(key);
        }

        public T Get(TID key)
        {
            _repositoryAsserter.AssertFound(key, _repository.ContainsKey(key));

            return _repository[key];
        }

        public bool Contains(TID key)
        {
            return _repository.ContainsKey(key);
        }
    }
}