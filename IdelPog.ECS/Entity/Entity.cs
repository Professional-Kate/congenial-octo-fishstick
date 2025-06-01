using IdelPog.ECS.Assertions;
using IdelPog.ECS.Component;
using IdelPog.Infrastructure.Repository;
using IdelPog.Infrastructure.Structures;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.ECS
{
    public abstract record Entity : IEntity
    {
        private readonly IRepository<Type, IComponent> _componentRepository;
        private readonly AssertComponentDoesNotExist _assertComponentDoesNotExist;
        private readonly AssertComponentFound _assertComponentFound;

        protected Entity(IRepository<Type, IComponent> components, IHandler handler)
        {
            _componentRepository = components;
            
            _assertComponentDoesNotExist = new AssertComponentDoesNotExist(handler);
            _assertComponentFound = new AssertComponentFound(handler);
        }

        /// <summary>
        /// Use <see cref="AddComponent"/> in this method to create always present <see cref="Component"/>s
        /// </summary>
        /// <remarks>
        /// Calling the base is currently not required. There are no base required components
        /// </remarks>
        protected virtual void AddRequiredComponents()
        {
            // No required base components
        }

        public void AddComponent(IComponent component)
        {
            _assertComponentDoesNotExist.Handle(_componentRepository.Contains(component.GetType()), component);
            
            _componentRepository.Add(component.GetType(), component);
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            _assertComponentFound.Handle(_componentRepository.Contains(typeof(T)), typeof(T));
            
            _componentRepository.Remove(typeof(T));
        }

        public T GetComponent<T>() where T : IComponent
        {
            _assertComponentFound.Handle(_componentRepository.Contains(typeof(T)), typeof(T));

            return (T) _componentRepository.Get(typeof(T));
        }

        public Optional<T> TryGetComponent<T>() where T : class, IComponent
        {
            bool contains = _componentRepository.Contains(typeof(T));

            if (contains == false)
            {
                return Optional<T>.None;
            }
            
            T component = (T) _componentRepository.Get(typeof(T));
            return new Optional<T>(component);
        }
    }
}