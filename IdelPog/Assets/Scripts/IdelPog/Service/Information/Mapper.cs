using System.Collections.Generic;
using IdelPog.Exceptions;
using IdelPog.Structures;

namespace IdelPog.Service
{
    public class Mapper<T> : IMapper<T>
    {
        private readonly Dictionary<T, Information> _information = new();
        
        public Information GetInformation(T key)
        {
            AssertKeyIsValid(key);

            return _information[key];
        }

        public void AddInformation(T key, Information information)
        {
            AssertKeyIsValid(key);
            
            _information.Add(key, information);
        }

        /// <summary>
        /// Asserts that the passed enum type hash code isn't zero
        /// </summary>
        /// <param name="key">The key you want to validate</param>
        /// <exception cref="NoTypeException">Will be thrown if the passed key's hash code is 0</exception>
        private static void AssertKeyIsValid(T key)
        {
            if (key.GetHashCode() == 0)
            {
                throw new NoTypeException("Error! Passed key is NO_TYPE, nothing can be added.");
            }
        }
    }
}