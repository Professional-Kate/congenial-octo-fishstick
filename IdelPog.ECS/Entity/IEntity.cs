using System.ComponentModel;

namespace IdelPog.ECS.Entity
{
    public interface IEntity
    {
        public void AddComponent(IComponent component);
        
        public void RemoveComponent<T>() where T : IComponent;
        
        public T GetComponent<T>() where T : IComponent;
        
        public bool TryGetComponent<T>(out T component) where T : IComponent;
    }
}