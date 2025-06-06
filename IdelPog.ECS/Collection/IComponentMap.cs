using IdelPog.ECS.Component;

namespace IdelPog.ECS.Collection
{
    public interface IComponentMap
    {
        public void Add(params IComponent[] components);
        
        public void Remove<T>();
        
        public IComponent Get<T>();
        
        public bool Contains<T>();

        public bool Contains(IComponent component);
    }
}