using System.Collections.Generic;
using IdelPog.Structures;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Interfaces;

namespace IdelPog.Service
{
    public class Mapper<T> : IMapper<T>
    {
        private readonly Dictionary<T, Information> _information = new();
        private readonly IAssertFound _assertFound;
        private readonly IAssertUniqueItem _assertUnique;

        public Mapper(IAssertFound assertFound, IAssertUniqueItem assertUnique)
        {
            _assertFound = assertFound;
            _assertUnique = assertUnique;
        }
        
        public Information GetInformation(T key)
        {
            bool contains = _information.TryGetValue(key, out Information information);
            _assertFound.AssertItemIsFound(key, () => contains == false);
            
            return information;
        }

        public void AddInformation(T key, Information information)
        {
            _assertUnique.AssertUnique(key, () => _information.ContainsKey(key));
            
            _information.Add(key, information);
        }
    }
}