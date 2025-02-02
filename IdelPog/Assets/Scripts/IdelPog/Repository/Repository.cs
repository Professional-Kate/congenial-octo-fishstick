using System;
using System.Collections.Generic;
using IdelPog.Exceptions;
using IdelPog.Structures;

namespace IdelPog.Repository
{
    public sealed class Repository<TID, T> : Singleton<Repository<TID, T>>, IRepository<TID, T> where T : class, ICloneable
    {
        private readonly Dictionary<TID, T> _repository = new();
        
        private Repository() { }
       
        public event Action<int, T> OnAdd;
        public event Action<int, T> OnRemove;
        public event Action<int, T> OnGet;
        public event Action<T, T> OnUpdate;
        public event Action<int, bool> OnContains;
        
        public void Add(TID key, T value)
        {
            AssertKeyIsValid(key);

            if (value == null)
            {
                throw new ArgumentNullException();
            }
            
            AssertKeyDoesNotExist(key);
            
            _repository.Add(key, value);
            OnAdd?.Invoke(key.GetHashCode(), value);
        }

        public void Remove(TID key)
        {
            AssertKeyIsValid(key);
            AssertKeyExists(key);
            
            T item = _repository[key];
            
            _repository.Remove(key);
            OnRemove?.Invoke(key.GetHashCode(), item);
        }

        public T Get(TID key)
        {
            AssertKeyIsValid(key);
            AssertKeyExists(key);
            
            T entity = _repository[key].Clone() as T;
            
            OnGet?.Invoke(key.GetHashCode(), entity);
            return entity;
        }

        public void Update(TID key, T value)
        {
            AssertKeyIsValid(key);
                
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            
            AssertKeyExists(key);
            T original  = _repository[key];
            
            _repository[key] = value;
            OnUpdate?.Invoke(original, value);
        }

        public bool Contains(TID key)
        {
            AssertKeyIsValid(key);

            bool contains = _repository.ContainsKey(key);
            
            OnContains?.Invoke(key.GetHashCode(), contains);
            
            return contains;
        }

        public void Clear()
        {
            _repository.Clear();
        }

        /// <summary>
        /// Asserts that the passed enum type hash code isn't zero
        /// </summary>
        /// <param name="key">The key you want to validate</param>
        /// <exception cref="NoTypeException">Will be thrown if the passed key's hash code  is 0</exception>
        private static void AssertKeyIsValid(TID key)
        {
            if (key.GetHashCode() == 0)
            {
                throw new NoTypeException();
            }
        }

        /// <summary>
        /// Asserts that the passed key is inside the Repository
        /// </summary>
        /// <param name="key">The key you want to check if it's in the Repository</param>
        /// <exception cref="NotFoundException">Will be thrown if the passed key is not in the Repository</exception>
        private void AssertKeyExists(TID key)
        {
            bool contains = _repository.ContainsKey(key);
            if (contains == false)
            {
                throw new NotFoundException($"Error! Passed key {key} is not in the Repository.");
            }
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