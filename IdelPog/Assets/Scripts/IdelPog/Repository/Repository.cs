using System;
using System.Collections.Generic;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Interfaces;

namespace IdelPog.Repository
{
    public sealed class Repository<TID, T> : IRepository<TID, T> where T : class, ICloneable
    {
        private readonly Dictionary<TID, T> _repository = new();
        private readonly IAssertFound _assertFound;
        private readonly IAssertNotNull _assertNotNull;
       
        public event Action<int, T> OnAdd;
        public event Action<int, T> OnRemove;
        public event Action<int, T> OnGet;
        public event Action<T, T> OnUpdate;
        public event Action<int, bool> OnContains;

        public Repository(IAssertFound assertFound, IAssertNotNull assertNotNull)
        {
            _assertFound = assertFound;
            _assertNotNull = assertNotNull;
        }
        
        public void Add(TID key, T value)
        {
            _assertNotNull.AssertObjectNotNull(value);
            AssertKeyDoesNotExist(key);
            
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
            _assertNotNull.AssertObjectNotNull(value);
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
            _assertFound.AssertItemIsFound(_repository.ContainsKey(key), key);
        }

        /// <summary>
        /// Asserts that the passed key is not inside the Repository
        /// </summary>
        /// <param name="key">The key you want to check if it's not in the Repository</param>
        /// <exception cref="ArgumentException">Will be thrown if the passed key is in the Repository</exception>
        private void AssertKeyDoesNotExist(TID key)
        {
            bool contains = _repository.ContainsKey(key);
            if (contains)
            {
                throw new ArgumentException($"Error! Passed key {key} already exists in the Repository. Cannot add!");
            }
        }
    }
}