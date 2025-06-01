using IdelPog.ECS.Component;
using IdelPog.ECS.Exceptions;
using IdelPog.Infrastructure.Structures;

namespace IdelPog.ECS
{
    /// <summary>
    /// Represents an entity in the ECS system, capable of holding and interacting with components
    /// </summary>
    /// <remarks>
    /// Entities manage and organise component lifecycle internally. Components are expected to be unique per type, no duplicate types are allowed
    /// </remarks>
    public interface IEntity
    {
        /// <summary>
        /// Adds a component to this entity
        /// </summary>
        /// <param name="component">The component to add</param>
        /// <exception cref="ComponentAlreadyExistsException"> Thrown if a component of the same type already exists on this entity </exception>
        public void AddComponent(IComponent component);
        
        /// <summary>
        /// Removes a component from this entity by type
        /// </summary>
        /// <typeparam name="T">The component type to remove</typeparam>
        /// <exception cref="ComponentNotFoundException"> Thrown if the component of type <typeparamref name="T"/> does not exist on this entity </exception>
        public void RemoveComponent<T>() where T : IComponent;
        
        /// <summary>
        /// Retrieves a component from this entity by type
        /// </summary>
        /// <typeparam name="T">The component type to retrieve</typeparam>
        /// <returns>The requested component</returns>
        /// <exception cref="ComponentNotFoundException"> Thrown if the component of type <typeparamref name="T"/> does not exist on this entity </exception>
        public T GetComponent<T>() where T : IComponent;

        /// <summary>
        /// Attempts to retrieve a component from this entity by type
        /// </summary>
        /// <typeparam name="T">The component type to retrieve</typeparam>
        /// <returns>
        /// An <see cref="Optional{T}"/> containing the component if it exists, or empty if not found
        /// </returns>
        public Optional<T> TryGetComponent<T>() where T : class, IComponent;
    }
}