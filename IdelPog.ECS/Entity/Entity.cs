using IdelPog.ECS.Assertions;
using IdelPog.ECS.Collection;
using IdelPog.ECS.Component;
using IdelPog.Infrastructure.Structures;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.ECS
{
    public abstract record Entity : IEntity
    {
        private readonly IComponentMap _componentMap;
        private readonly AssertComponentDoesNotExist _assertComponentDoesNotExist;
        private readonly AssertComponentFound _assertComponentFound;

        protected Entity(params IComponent[] requiredComponents)
        {
            _componentMap = new ComponentMap();
            _assertComponentFound = new AssertComponentFound(new ThrowHandler());
            _assertComponentDoesNotExist = new AssertComponentDoesNotExist(new ThrowHandler());
            
            _componentMap.Add(requiredComponents);
        }

        protected Entity(IComponentMap componentMap, IHandler handler)
        {
            _componentMap = componentMap;
            
            _assertComponentDoesNotExist = new AssertComponentDoesNotExist(handler);
            _assertComponentFound = new AssertComponentFound(handler);
        }

        public void AddComponent(IComponent component)
        {
            _assertComponentDoesNotExist.Handle(_componentMap.Contains(component), component);
            
            _componentMap.Add(component);
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            _assertComponentFound.Handle(_componentMap.Contains<T>(), typeof(T));
            
            _componentMap.Remove<T>();
        }

        public IComponent GetComponent<T>() where T : IComponent
        {
            _assertComponentFound.Handle(_componentMap.Contains<T>(), typeof(T));

            return _componentMap.Get<T>();
        }

        public Optional<T> TryGetComponent<T>() where T : IComponent
        {
            bool contains = _componentMap.Contains<T>();

            if (contains == false)
            {
                return Optional<T>.None;
            }
            
            T component = (T) _componentMap.Get<T>();
            return new Optional<T>(component);
        }
    }
}