using System;
using UnityEngine;

namespace Injector.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class Inject : Attribute
    {
        private readonly DependencyInjector _injector = DependencyInjector.GetInstance();

        public Inject()
        {
            Debug.Log("INJECT");
            _injector.InjectComponent(GetType());
        }
    }
}