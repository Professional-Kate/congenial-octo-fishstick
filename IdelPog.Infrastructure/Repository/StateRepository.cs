using IdelPog.Infrastructure.Structures;

namespace IdelPog.Infrastructure.Repository
{
    public sealed class StateRepository<TID, T>(IRepositoryAsserter repositoryAsserter) : IRepository<TID, T>
        where T : class, ICloneable<T> where TID : notnull
    {
        private readonly Dictionary<TID, T> _repository = new();
        
        public void Add(TID key, T value)
        {
            repositoryAsserter.AssertUnique(value, () => _repository.ContainsKey(key));
            
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
            repositoryAsserter.AssertFound(key, () => _repository.ContainsKey(key));
        }
    }
}