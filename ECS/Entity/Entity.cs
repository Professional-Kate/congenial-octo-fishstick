using System.Diagnostics.CodeAnalysis;
using IdelPog.ECS.Assertion;
using IdelPog.ECS.Assertion.Interface;
using IdelPog.ECS.Component;

namespace IdelPog.ECS.Entity
{
    public abstract record Entity : IEntity
    {
        private readonly IDictionary<Type, IComponent> _componentMap;
        private readonly IComponentAssertion _componentAssertion;

        protected Entity(params IComponent[] requiredComponents)
        {
            _componentAssertion = new ComponentAssertion();
            _componentMap = new Dictionary<Type, IComponent>();

            foreach (IComponent requiredComponent in requiredComponents)
            {
                AddComponent(requiredComponent);
            }
        }

        protected Entity(IDictionary<Type, IComponent> componentMap)
        {
            _componentMap = componentMap;

            _componentAssertion = new ComponentAssertion();
        }

        public void AddComponent(IComponent component)
        {
            _componentAssertion.AssertUnique<IComponent>(_componentMap.ContainsKey(component.GetType()));
            
            _componentMap.Add(component.GetType(), component);
        }

        public void RemoveComponent<TComponent>() where TComponent : IComponent
        {
            _componentAssertion.AssertFound<TComponent>(_componentMap.ContainsKey(typeof(TComponent)));

            _componentMap.Remove(typeof(TComponent));
        }

        public bool ContainsComponent<TComponent>() where TComponent : IComponent
        {
            return _componentMap.ContainsKey(typeof(TComponent));
        }

        public TComponent GetComponent<TComponent>() where TComponent : IComponent
        {
            _componentAssertion.AssertFound<TComponent>(_componentMap.ContainsKey(typeof(TComponent)));
            
            return (TComponent) _componentMap[typeof(TComponent)];
        }

        public bool TryGetComponent<T>([NotNullWhen(true)] out T? component) where T : IComponent
        {
            bool contains = _componentMap.ContainsKey(typeof(T));

            if (contains == false)
            {
                component = default;
                return false;
            }

            component = (T) _componentMap[typeof(T)];
            return true;
        }
    }
}