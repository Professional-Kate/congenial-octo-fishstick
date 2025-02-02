using System;
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
            
            bool contains = _information.TryGetValue(key, out Information information);
            if (contains == false)
            {
                throw new NotFoundException($"Error! Key {key} was not found in the Dictionary!");
            }
            
            return information;
        }

        public void AddInformation(T key, Information information)
        {
            AssertKeyIsValid(key);

            bool contains = _information.ContainsKey(key);
            if (contains)
            {
                throw new ArgumentException($"Error! Passed Key {key} is already in the Dictionary!");
            }
            
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
                throw new NoTypeException();
            }
        }
    }
}