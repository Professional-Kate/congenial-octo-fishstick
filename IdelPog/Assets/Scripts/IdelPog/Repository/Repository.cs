using System;
using System.Collections.Generic;
using IdelPog.Exceptions;
using IdelPog.Structures.Enums;

namespace IdelPog.Repository
{
    public sealed class Repository<TID, T> : IRepository<TID, T> where T : class, ICloneable
    {
        private readonly Dictionary<TID, T> _repository = new();
        
        public event Action<RepositoryOperation, TID, T> OnRepositoryChange;
        public event Action<RepositoryOperation, TID, bool> OnContains;

        public void Add(TID key, T value)
        {
            AssertKeyIsValid(key);

            if (value == null)
            {
                throw new ArgumentNullException();
            }
            
            AssertKeyDoesNotExist(key);
            
            _repository.Add(key, value);
            OnRepositoryChange?.Invoke(RepositoryOperation.ADD, key, value);
        }

        public void Remove(TID key)
        {
            AssertKeyIsValid(key);
            AssertKeyExists(key);
            
            T item = _repository[key];
            
            _repository.Remove(key);
            OnRepositoryChange?.Invoke(RepositoryOperation.REMOVE, key, item);
        }

        public T Get(TID key)
        {
            AssertKeyIsValid(key);
            AssertKeyExists(key);
            
            T entity = _repository[key].Clone() as T;
            
            OnRepositoryChange?.Invoke(RepositoryOperation.GET, key, entity);
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
            
            _repository[key] = value;
            OnRepositoryChange?.Invoke(RepositoryOperation.UPDATE, key, value);
        }

        public bool Contains(TID key)
        {
            AssertKeyIsValid(key);

            bool contains = _repository.ContainsKey(key);
            
            OnContains?.Invoke(RepositoryOperation.CONTAINS, key, contains);
            
            return contains;
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
                throw new NoTypeException($"Error! Passed Key : {key} is NO_TYPE, nothing can be retrieved. This should be fixed.");
            }
        }

        /// <summary>
        /// Asserts that the passed key is inside the Repository
        /// </summary>
        /// <param name="key">The key you want to check if it's in the Repository</param>
        /// <exception cref="NotFoundException">Will be thrown if the passed key is not in the Repository</exception>
        private void AssertKeyExists(TID key)
        {
            bool contains = Contains(key);
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