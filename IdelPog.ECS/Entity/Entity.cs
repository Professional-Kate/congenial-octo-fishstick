using IdelPog.ECS.Assertions;
using IdelPog.ECS.Collection;
using IdelPog.ECS.Component;
using IdelPog.Infrastructure.Repository;
using IdelPog.Infrastructure.Structures;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.ECS
{
    public abstract record Entity : IEntity
    {
        private readonly IAssetRepository<Type, IComponent> _componentMap;
        private readonly AssertComponentDoesNotExist _assertComponentDoesNotExist;
        private readonly AssertComponentFound _assertComponentFound;

        protected Entity(params IComponent[] requiredComponents)
        {
            _componentMap = new AssetRepository<Type, IComponent>();
            _assertComponentFound = new AssertComponentFound(new ThrowHandler());
            _assertComponentDoesNotExist = new AssertComponentDoesNotExist(new ThrowHandler());
            
            foreach (IComponent requiredComponent in requiredComponents)
            {
                AddComponent(requiredComponent);
            }
        }

        protected Entity(IAssetRepository<Type, IComponent> componentMap, IHandler handler)
        {
            _componentMap = componentMap;
            
            _assertComponentDoesNotExist = new AssertComponentDoesNotExist(handler);
            _assertComponentFound = new AssertComponentFound(handler);
        }

        public void AddComponent(IComponent component)
        {
            _assertComponentDoesNotExist.Handle(_componentMap.Contains(component.GetType()), component);
            
            _componentMap.Add(component.GetType(), component);
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            _assertComponentFound.Handle(_componentMap.Contains(typeof(T)), typeof(T));
            
            _componentMap.Remove(typeof(T));
        }

        public IComponent GetComponent<T>() where T : IComponent
        {
            _assertComponentFound.Handle(_componentMap.Contains(typeof(T)), typeof(T));

            return _componentMap.Get(typeof(T));
        }

        public Optional<T> TryGetComponent<T>() where T : IComponent
        {
            bool contains = _componentMap.Contains(typeof(T));

            if (contains == false)
            {
                return Optional<T>.None;
            }
            
            T component = (T) _componentMap.Get(typeof(T));
            return new Optional<T>(component);
        }
    }
}