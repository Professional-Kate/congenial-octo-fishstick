using IdelPog.Engine.Validation.Pipelines.Interfaces;

namespace IdelPog.Engine.Repository
{
    public sealed class Repository<TID, T>(IRepositoryAsserter repositoryAsserter) : IRepository<TID, T>
        where T : class, ICloneable
    {
        private readonly Dictionary<TID, T> _repository = new();

        public event Action<int, T>? OnAdd;
        public event Action<int, T>? OnRemove;
        public event Action<int, T>? OnGet;
        public event Action<T, T>? OnUpdate;
        public event Action<int, bool>? OnContains;

        public void Add(TID key, T value)
        {
            repositoryAsserter.AssertUnique(value, () => _repository.ContainsKey(key));
            
            _repository.Add(key, value);
            OnAdd?.Invoke(key.GetHashCode(), value);
        }

        public void Remove(TID key)
        {
            AssertKeyExists(key);
            
            T item = _repository[key];
            
            _repository.Remove(key);
            OnRemove?.Invoke(key.GetHashCode(), item);
        }

        public T Get(TID key)
        {
            AssertKeyExists(key);
            
            T entity = _repository[key].Clone() as T;
            
            OnGet?.Invoke(key.GetHashCode(), entity);
            return entity;
        }

        public void Update(TID key, T value)
        {
            AssertKeyExists(key);
            
            T original  = _repository[key];
            
            _repository[key] = value;
            OnUpdate?.Invoke(original, value);
        }

        public bool Contains(TID key)
        {
            bool contains = _repository.ContainsKey(key);
            
            OnContains?.Invoke(key.GetHashCode(), contains);
            
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