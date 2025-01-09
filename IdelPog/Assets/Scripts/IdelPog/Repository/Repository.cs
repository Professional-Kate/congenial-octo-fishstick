using System;
using System.Collections.Generic;
using IdelPog.Exceptions;

namespace IdelPog.Repository
{
    public class Repository<TID, T> : IRepository<TID, T>
    {
        private readonly Dictionary<TID, T> _repository = new();

        public virtual void Add(TID key, T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            
            AssertKeyDoesNotExist(key);
            
            _repository.Add(key, value);
        }

        public virtual void Remove(TID key)
        {
            AssertKeyExists(key);
            
            _repository.Remove(key);
        }

        public virtual T Get(TID key)
        {
            AssertKeyExists(key);
            
            T entity = _repository[key];
            return entity;
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