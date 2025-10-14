using IdelPog.Core.Contracts;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;

namespace IdelPog.Core.Repository.State
{
    public sealed class StateRepository<TID, T> : IStateRepository<TID, T>
        where T : class, ICloneable<T> where TID : notnull
    {
        private readonly Dictionary<TID, T> _repository = new();
        private readonly IRepositoryAsserter _repositoryAsserter;

        public StateRepository()
        {
            _repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());
        }

        public StateRepository(IRepositoryAsserter repositoryAsserter)
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
            AssertKeyExists(key);

            _repository.Remove(key);
        }

        public T Get(TID key)
        {
            AssertKeyExists(key);

            T entity = _repository[key].DeepClone();

            return entity;
        }

        public void Update(TID key, T value)
        {
            _repositoryAsserter.AssertNotNull(value);
            AssertKeyExists(key);

            _repository[key] = value;
        }

        public bool Contains(TID key)
        {
            bool contains = _repository.ContainsKey(key);

            return contains;
        }

        /// <summary>
        /// Asserts that the passed key is inside the Repository
        /// </summary>
        /// <param name="key">The key you want to check if it's in the Repository</param>
        private void AssertKeyExists(TID key)
        {
            _repositoryAsserter.AssertFound(key, _repository.ContainsKey(key));
        }
    }
}