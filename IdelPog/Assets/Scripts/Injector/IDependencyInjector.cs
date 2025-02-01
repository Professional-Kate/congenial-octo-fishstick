using System;

namespace Injector
{
    /// <summary>
    /// Use this interface to return a built Component
    /// </summary>
    /// <seeaalso cref="Get{T}"/>
    /// <see cref="RegisterComponent"/>
    public interface IDependencyInjector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeToRegister"></param>
        public void RegisterComponent(Type typeToRegister);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        public void InjectComponent(object component);
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : class;
    }
}