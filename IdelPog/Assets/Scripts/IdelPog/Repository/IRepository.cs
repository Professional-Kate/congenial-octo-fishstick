using System;
using IdelPog.Exceptions;

namespace IdelPog.Repository
{
    /// <summary>
    /// Generic Interface, the implementing class is expected to create its own data structure for each method to use
    /// </summary>
    /// <typeparam name="TID">The key to link to the value</typeparam>
    /// <typeparam name="T">The value to link with the key</typeparam>
    /// <seealso cref="Add"/>
    /// <seealso cref="Remove"/>
    /// <seealso cref="Get"/>
    public interface IRepository<in TID, T>
    {
        /// <summary>
        /// Adds a new Key Value pair into the Repository
        /// </summary>
        /// <param name="key">The key to link to the value</param>
        /// <param name="value">The value to link with the key</param>
        /// <exception cref="ArgumentException">Will be thrown if the passed value is null</exception>
        public void Add(TID key, T value);
        
        /// <summary>
        /// Removes a Key Value pair from the Repository
        /// </summary>
        /// <param name="key">The key to remove from the Repository</param>
        /// <exception cref="NotFoundException">Will be thrown if the passed key is not in the Repository</exception>
        public void Remove(TID key);

        /// <summary>
        /// Get an item from the repository
        /// </summary>
        /// <param name="key">The value you wanted will have this key</param>
        /// <returns>The found value</returns>
        /// <exception cref="NotFoundException">Will be thrown if the passed key is not in the Repository</exception>
        public T Get(TID key);
    }
}