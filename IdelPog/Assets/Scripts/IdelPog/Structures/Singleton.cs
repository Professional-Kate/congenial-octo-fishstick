using System;
using System.Reflection;

namespace IdelPog.Structures
{
    /// <summary>
    /// Inherit from this class to create a Singleton from the inheriting class. Calling <see cref="GetInstance"/> will always return the instance of that class.
    /// </summary>
    /// <typeparam name="T">The inheriting class</typeparam>
    /// <seealso cref="GetInstance"/>
    public class Singleton<T> where T : class
    {
        private static T _instance;

        /// <summary>
        /// Returns an instance of the passed type T
        /// </summary>
        /// <returns>An instance of type T</returns>
        /// <remarks>
        ///  Is expected to always return an instance of type T. If the instance does not exist then this will create one and return it
        /// </remarks>
        public static T GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            
            ConstructorInfo constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            if (constructor == null)
            {
                throw new MissingMethodException($"The passed type {typeof(T)} does not have a private constructor.");
            }
                
            _instance = (T) constructor.Invoke(null);
            return _instance;
        }
    }
}