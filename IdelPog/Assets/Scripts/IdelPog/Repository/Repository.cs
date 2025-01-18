using System;
using System.Collections.Generic;
using IdelPog.Exceptions;

namespace IdelPog.Repository
{
    public sealed class Repository<TID, T> : IRepository<TID, T> where T : class, ICloneable
    {
        private readonly Dictionary<TID, T> _repository = new();

        public void Add(TID key, T value)
        {
            AssertKeyIsValid(key);

            if (value == null)
            {
                throw new ArgumentNullException();
            }
            
            AssertKeyDoesNotExist(key);
            
            _repository.Add(key, value);
        }

        public void Remove(TID key)
        {
            AssertKeyIsValid(key);
            AssertKeyExists(key);
            
            _repository.Remove(key);
        }

        public T Get(TID key)
        {
            AssertKeyIsValid(key);
            AssertKeyExists(key);
            
            T entity = _repository[key];
            
            return entity.Clone() as T;
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