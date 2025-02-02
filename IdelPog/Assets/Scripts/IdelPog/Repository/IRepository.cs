using System;
using IdelPog.Exceptions;

namespace IdelPog.Repository
{
    /// <summary>
    /// Generic Repository, the implementing class is expected to create its own data structure for each method to use. This interface gives basic CRUD access to data
    /// </summary>
    /// <typeparam name="TID">The key to link to the value</typeparam>
    /// <typeparam name="T">The value to link with the key</typeparam>
    /// <seealso cref="Add"/>
    /// <seealso cref="Remove"/>
    /// <seealso cref="Get"/>
    /// <seealso cref="Update"/>
    /// <seealso cref="Contains"/>
    public interface IRepository<in TID, T>  : IRepositoryHooks<T>
    {
        /// <summary>
        /// Adds a new Key Value pair into the Repository
        /// </summary>
        /// <param name="key">The key to link to the value</param>
        /// <param name="value">The value to link with the key</param>
        /// <exception cref="ArgumentNullException">Will be thrown if the passed value is null</exception>
        /// <exception cref="ArgumentException">If the passed key value pair is already in the Repository</exception>
        /// <exception cref="NoTypeException">Will be thrown if the passed key is NO_TYPE</exception>
        public void Add(TID key, T value);
        
        /// <summary>
        /// Removes a Key Value pair from the Repository
        /// </summary>
        /// <param name="key">The key to remove from the Repository</param>
        /// <exception cref="NotFoundException">Will be thrown if the passed key is not in the Repository</exception>
        /// <exception cref="NoTypeException">Will be thrown if the passed key is NO_TYPE</exception>
        public void Remove(TID key);

        /// <summary>
        /// Get an item from the Repository
        /// </summary>
        /// <param name="key">The value you wanted will be linked to this key</param>
        /// <returns>The found value</returns>
        /// <exception cref="NotFoundException">Will be thrown if the passed key is not in the Repository</exception>
        /// <exception cref="NoTypeException">Will be thrown if the passed key is NO_TYPE</exception>
        /// <remarks>
        /// This will return an object by reference.  
        /// </remarks>
        public T Get(TID key);
        
        /// <summary>
        /// Updates a value in the Repository. The passed value will completely replace the one inside the Repository
        /// </summary>
        /// <param name="key">The value you want to update will be linked to this key</param>
        /// <param name="value">The new value of the object linked to the key</param>
        /// <exception cref="ArgumentNullException">Will be thrown if the passed value is null</exception>
        /// <exception cref="NotFoundException">Will be thrown if the key cannot be found in the Repository</exception>
        /// <exception cref="NoTypeException">Will be thrown if the passed key is NO_TYPE</exception>
        public void Update(TID key, T value);
        
        /// <summary>
        /// Will check if the passed key is inside the Repository
        /// </summary>
        /// <param name="key">The key you want to check</param>
        /// <returns>A boolean on if the requested key exists or not</returns>
        /// <exception cref="NoTypeException">Will be thrown if the passed key is NO_TYPE</exception>
        public bool Contains(TID key);

        /// <summary>
        /// Clear the entire Repository, deleting all items inside
        /// </summary>
        public void Clear();
    }
}