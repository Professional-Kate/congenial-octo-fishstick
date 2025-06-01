using IdelPog.ECS.Assertions;
using IdelPog.ECS.Exceptions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.ECS.Component.Store
{
    /// <summary>
    /// A component store used to group related <see cref="IComponent"/> instances of the same type
    /// </summary>
    /// <typeparam name="T">The component type this store holds. Must implement <see cref="IComponent"/></typeparam>
    /// <remarks>
    /// This store returns component references using <see cref="GetAllComponents"/>.
    /// Consumers are expected to mutate the components in place if needed
    /// </remarks>
    public sealed class ComponentStore<T> : IComponent where T : IComponent
    {
        private readonly List<T> _components = [];
        private readonly AssertComponentDoesNotExist _assertComponentDoesNotExist;
        private readonly AssertComponentFound _assertComponentFound;

        public ComponentStore(IHandler handler)
        {
            _assertComponentDoesNotExist = new AssertComponentDoesNotExist(handler);
            _assertComponentFound = new AssertComponentFound(handler);
        }

        /// <summary>
        /// Add a component to the store. No duplicates will be allowed
        /// </summary>
        /// <param name="component">The component to add</param>
        /// <exception cref="ComponentAlreadyExistsException">Thrown if the component already exists in the store</exception>
        public void AddComponent(T component)
        {
            _assertComponentDoesNotExist.Handle(_components.Contains(component), component);
            
            _components.Add(component);
        }

        /// <summary>
        /// Remove a component from the store
        /// </summary>
        /// <param name="component">The component to remove</param>
        /// <exception cref="ComponentNotFoundException">Thrown if the component is not present in the store</exception>
        public void RemoveComponent(T component)
        {
            _assertComponentFound.Handle(_components.Contains(component), component.GetType());
            
            _components.Remove(component);
        }

        /// <summary>
        /// Returns all components from the store by reference.
        /// Mutations to the returned array element will affect the stored components
        /// </summary>
        /// <returns>An array of all stored components</returns>
        public T[] GetAllComponents()
        {
            return _components.ToArray();
        }
    }
}