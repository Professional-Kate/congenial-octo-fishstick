using IdelPog.ECS.Assertions;
using IdelPog.ECS.Exceptions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.ECS.Component
{
    /// <summary>
    /// A component store used to group related <see cref="IComponent{TComponent}"/> instances of the same type
    /// </summary>
    /// <typeparam name="T">The component type this store holds. Must implement <see cref="IComponent{TComponent}"/></typeparam>
    /// <remarks>
    /// This store returns cloned components using <see cref="GetAllComponents"/> to ensure immutability.
    /// Consumers are not expected to mutate the returned components in a way that affects the store
    /// </remarks>
    public readonly struct ComponentStore<T> : IComponent<ComponentStore<T>> where T : IComponent<T>
    {
        private readonly T[] _components;
        private readonly IHandler _handler;

        /// <summary>
        /// Creates a new store containing the provided components
        /// </summary>
        /// <param name="components">An array of components to store. Must not be null or empty</param>
        /// <param name="handler">Handler used for controlling assertion failure behavior</param>
        /// <exception cref="ComponentArrayNullException">Thrown if the passed components are null</exception>
        /// <exception cref="ComponentArrayEmptyException">Thrown if the passed components are empty </exception>
        public ComponentStore(T[] components, IHandler handler)
        {
            _handler = handler;
            AssertArrayNotEmpty assertArrayNotEmpty = new(_handler);
            AssertArrayNotNull assertArrayNotNull = new(_handler);
            
            assertArrayNotNull.Handle(components);
            assertArrayNotEmpty.Handle(components.Length > 0);
            
            _components = components;
        }

        /// <summary>
        /// Determined whether any component in the store matches the given predicate  
        /// </summary>
        /// <param name="predicate">The Predicate to run each component against</param>
        /// <returns>if any component satisfied the predicate</returns>
        public bool ContainsComponent(Predicate<T> predicate)
        {
            foreach (T component in _components)
            {
                if (predicate(component))
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Returns deep clones of all stored components.
        /// Consumer can safely mutate the returned array without affecting internal state 
        /// </summary>
        ///<remarks>
        /// Each individual component determines how much of its internal state is cloned.
        /// Any data can be excluded from the clone if the component chooses to hide it
        /// </remarks>
        /// <returns>An array of cloned components</returns>
        public T[] GetAllComponents()
        {
            T[] clones = new T[_components.Length];

            for (int i = 0; i < _components.Length; i++)
            {
                clones[i] = _components[i].DeepClone();    
            }
            
            return clones;
        }

        public ComponentStore<T> DeepClone()
        {
            return new ComponentStore<T>(GetAllComponents(), _handler);
        }
    }
}