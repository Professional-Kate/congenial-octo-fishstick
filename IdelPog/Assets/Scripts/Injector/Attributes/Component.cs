using System;

namespace Injector.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class Component : Attribute
    {
        private static readonly DependencyInjector _dependencyInjector = DependencyInjector.GetInstance();
        
        public Component()
        {
            _dependencyInjector.RegisterComponent(GetType());
        }
    }
}