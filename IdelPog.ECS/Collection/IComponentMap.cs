using IdelPog.ECS.Component;

namespace IdelPog.ECS.Collection
{
    public interface IComponentMap
    {
        public void Add(IComponent component);
        
        public void Add(IComponent[] components);
        
        public void Remove<T>();
        
        public IComponent Get<T>();
        
        public bool Contains<T>();

        public bool Contains(IComponent component);
    }
}