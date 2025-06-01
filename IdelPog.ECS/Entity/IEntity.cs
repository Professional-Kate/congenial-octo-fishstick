using IdelPog.ECS.Component;
using IdelPog.Infrastructure.Structures;

namespace IdelPog.ECS
{
    public interface IEntity
    {
        public void AddComponent(IComponent component);
        
        public void RemoveComponent<T>() where T : IComponent;
        
        public T GetComponent<T>() where T : IComponent;

        public Optional<T> TryGetComponent<T>() where T : class, IComponent;
    }
}