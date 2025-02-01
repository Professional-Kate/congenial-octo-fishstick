using System;
using System.Collections.Generic;
using IdelPog.Structures;

namespace IdelPog.Service
{
    public class Mapper<T> : IMapper<T>
    {
        protected Dictionary<T, Information> Informations = new();
        
        public Information GetInformation(T key)
        {
            throw new NotImplementedException();
        }
    }
}