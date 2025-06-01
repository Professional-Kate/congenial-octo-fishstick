using IdelPog.ECS.Component;
using IdelPog.Infrastructure.Repository;
using IdelPog.Infrastructure.Structures;

namespace IdelPog.ECS
{
    public abstract record Entity : IEntity
    {
        private readonly IRepository<Type, IComponent> _componentRepository;

        protected Entity(IRepository<Type, IComponent> components)
        {
            _componentRepository = components;
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
            if (_componentRepository.Contains(component.GetType()))
            {
                throw new Exception();
            }
            
            _componentRepository.Add(component.GetType(), component);
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            if (_componentRepository.Contains(typeof(T)) == false)
            {
                throw new Exception();
            }
            
            _componentRepository.Remove(typeof(T));
        }

        public T GetComponent<T>() where T : IComponent
        {
            bool contains = _componentRepository.Contains(typeof(T));

            if (contains == false)
            {
                throw new Exception();
            }

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