using IdelPog.Infrastructure.Structures;

namespace IdelPog.Infrastructure.Repository
{
    public sealed class Repository<TID, T>(IRepositoryAsserter repositoryAsserter) : IRepository<TID, T>
        where T : class, ICloneable<T> where TID : notnull
    {
        private readonly Dictionary<TID, T> _repository = new();

        public event Action<int, T> OnAdd = delegate { };
        public event Action<int, T> OnRemove = delegate { };
        public event Action<int, T> OnGet = delegate { };
        public event Action<T, T> OnUpdate = delegate { };
        public event Action<int, bool> OnContains = delegate { };

        public void Add(TID key, T value)
        {
            repositoryAsserter.AssertUnique(value, () => _repository.ContainsKey(key));
            
            _repository.Add(key, value);
            OnAdd(key.GetHashCode(), value);
        }

        public void Remove(TID key)
        {
            AssertKeyExists(key);
            
            T item = _repository[key];
            
            _repository.Remove(key);
            OnRemove(key.GetHashCode(), item);
        }

        public T Get(TID key)
        {
            AssertKeyExists(key);
            
            T entity = _repository[key].Clone();
            
            OnGet(key.GetHashCode(), entity);
            return entity;
        }

        public void Update(TID key, T value)
        {
            AssertKeyExists(key);
            
            T original  = _repository[key];
            
            _repository[key] = value;
            OnUpdate(original, value);
        }

        public bool Contains(TID key)
        {
            bool contains = _repository.ContainsKey(key);
            
            OnContains(key.GetHashCode(), contains);
            
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