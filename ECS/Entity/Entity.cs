using System.Diagnostics.CodeAnalysis;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.ECS.Assertion;
using IdelPog.ECS.Assertion.Interface;
using IdelPog.ECS.Component;

namespace IdelPog.ECS.Entity
{
    public abstract record Entity : IEntity
    {
        private readonly IAssetRepository<Type, IComponent> _componentMap;
        private readonly IComponentAssertion _componentAssertion;

        protected Entity(params IComponent[] requiredComponents)
        {
            _componentMap = new AssetRepository<Type, IComponent>();
            _componentAssertion = new ComponentAssertion(new ThrowHandler());

            foreach (IComponent requiredComponent in requiredComponents)
            {
                AddComponent(requiredComponent);
            }
        }

        protected Entity(IAssetRepository<Type, IComponent> componentMap, IHandler handler)
        {
            _componentMap = componentMap;

            _componentAssertion = new ComponentAssertion(handler);
        }

        public void AddComponent(IComponent component)
        {
            _componentAssertion.AssertUnique<IComponent>(_componentMap.Contains(component.GetType()));

            _componentMap.Add(component.GetType(), component);
        }

        public void RemoveComponent<TComponent>() where TComponent : IComponent
        {
            _componentAssertion.AssertFound<TComponent>(_componentMap.Contains(typeof(TComponent)));

            _componentMap.Remove(typeof(TComponent));
        }

        public bool ContainsComponent<TComponent>() where TComponent : IComponent
        {
            return _componentMap.Contains(typeof(TComponent));
        }

        public TComponent GetComponent<TComponent>() where TComponent : IComponent
        {
            _componentAssertion.AssertFound<TComponent>(_componentMap.Contains(typeof(TComponent)));

            return (TComponent)_componentMap.Get(typeof(TComponent));
        }

        public bool TryGetComponent<T>([NotNullWhen(true)] out T? component) where T : IComponent
        {
            bool contains = _componentMap.Contains(typeof(T));

            if (contains == false)
            {
                component = default;
                return false;
            }

            component = (T)_componentMap.Get(typeof(T));
            return true;
        }
    }
}