using System;
using System.Collections.Generic;
using IdelPog.Structures;

namespace IdelPog.Service
{
    public abstract class Mapper<T> : IMapper<T>
    {
        protected Dictionary<T, Information> Informations = new();
        
        public Information GetInformation(T key)
        {
            throw new NotImplementedException();
        }
    }
}