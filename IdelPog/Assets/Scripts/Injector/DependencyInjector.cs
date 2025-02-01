using System;
using System.Collections.Generic;
using IdelPog.Exceptions;
using IdelPog.Structures;

namespace Injector
{
    /// <inheritdoc cref="IDependencyInjector"/>
    public class DependencyInjector : Singleton<DependencyInjector>, IDependencyInjector
    {
        private readonly Dictionary<Type, object> _dependencies = new();
        
        private DependencyInjector() { }
      
        public void RegisterComponent(Type typeToRegister)
        {
            if (_dependencies.ContainsKey(typeToRegister))
            {
                throw new ArgumentException($"The type {typeToRegister} has already been registered.");
            }
            
            object instance = Activator.CreateInstance(typeToRegister);
            _dependencies.Add(typeToRegister, instance);
        }

        public void InjectComponent(object component)
        {
            throw new NotImplementedException();
        }
        
        public T Get<T>() where T : class
        {
            bool contains = _dependencies.TryGetValue(typeof(T), out var instance);
            if (contains == false)
            {
                throw new NotFoundException($"The passed type {typeof(T)} does not exist.");
            }
            
            return (T) instance; 
        }

    }
}