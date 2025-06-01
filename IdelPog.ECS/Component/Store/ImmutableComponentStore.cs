namespace IdelPog.ECS.Component.Store
{
    /// <summary>
    /// A component store used to group related <see cref="IComponent"/> instances of the same type
    /// </summary>
    /// <typeparam name="T">The component type this store holds. Must implement <see cref="ICloneableComponent{T}"/></typeparam>
    /// <remarks>
    /// This store returns cloned components using <see cref="GetAllComponents"/> to ensure immutability.
    /// Consumers are not expected to mutate the returned components in a way that affects the store
    /// </remarks>
    public sealed class ImmutableComponentStore<T> : IComponent where T : ICloneableComponent<T>
    {
        private readonly T[] _components;
        
        /// <summary>
        /// Creates a new store containing the provided components
        /// </summary>
        /// <param name="components">An array of components to store. Must not be null or empty</param>
        /// <exception cref="ArgumentNullException">Thrown if the passed components are null</exception>
        /// <exception cref="Exception">Thrown if the passed components are empty </exception>
        public ImmutableComponentStore(T[] components)
        {
            ArgumentNullException.ThrowIfNull(components);
            
            if (components.Length == 0)
            {
                throw new Exception();
            }
            
            _components = components.ToArray();
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
                clones[i] = _components[i].Clone();    
            }
            
            return clones;
        }
    }
}