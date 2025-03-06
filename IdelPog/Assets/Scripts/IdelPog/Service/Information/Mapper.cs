using System;
using System.Collections.Generic;
using IdelPog.Structures;
using IdelPog.Validation;

namespace IdelPog.Service
{
    public class Mapper<T> : IMapper<T>
    {
        private readonly Dictionary<T, Information> _information = new();
        
        public Information GetInformation(T key)
        {
            bool contains = _information.TryGetValue(key, out Information information);
            if (contains == false)
            {
                throw new NotFoundException(key, GetType());
            }
            
            return information;
        }

        public void AddInformation(T key, Information information)
        {
            bool contains = _information.ContainsKey(key);
            if (contains)
            {
                throw new ArgumentException($"Error! Passed Key {key} is already in the Dictionary!");
            }
            
            _information.Add(key, information);
        }
    }
}