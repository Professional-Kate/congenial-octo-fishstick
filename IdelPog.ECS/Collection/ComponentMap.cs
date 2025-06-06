using IdelPog.ECS.Component;

namespace IdelPog.ECS.Collection
{
    public class ComponentMap : IComponentMap
    {
        private readonly Dictionary<Type, IComponent> _components = new();
        
        public void Add(IComponent component)
        {
            if (component == null)
            {
                throw new Exception();
            }
            
            if (_components.ContainsKey(component.GetType()))
            {
                throw new Exception();
            }
            
            _components.Add(component.GetType(), component);
        }

        public void Add(IComponent[] components)
        {
            if (components.Length == 0)
            {
                throw new Exception();
            }
            
            foreach (IComponent component in components)
            {
                if (component == null)
                {
                    throw new Exception();
                }

                if (_components.ContainsKey(component.GetType()))
                {
                    throw new Exception();
                }
                
                _components.Add(component.GetType(), component);
            }
        }

        public void Remove<T>()
        {
            if (_components.ContainsKey(typeof(T)) == false)
            {
                throw new Exception();
            }
            
            _components.Remove(typeof(T));
        }

        public IComponent Get<T>()
        {
            if (_components.ContainsKey(typeof(T)) == false) 
            {
                throw new Exception();
            }
            
            return _components[typeof(T)];
        }

        public bool Contains<T>()
        {
            return _components.ContainsKey(typeof(T));
        }

        public bool Contains(IComponent type)
        {
            return _components.ContainsKey(type.GetType());
        }
    }
}
