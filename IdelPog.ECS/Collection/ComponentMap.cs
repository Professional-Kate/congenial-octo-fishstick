using IdelPog.ECS.Assertions;
using IdelPog.ECS.Component;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.ECS.Collection
{
    public class ComponentMap : IComponentMap
    {
        private readonly Dictionary<Type, IComponent> _components = new();
        private readonly AssertComponentFound _assertComponentFound;
        private readonly AssertComponentDoesNotExist _assertComponentDoesNotExist;
        private readonly AssertArrayNotEmpty _assertArrayNotEmpty;
        private readonly AssertNotNull _assertNotNull;

        public ComponentMap()
        {
            _assertComponentFound = new AssertComponentFound(new ThrowHandler());
            _assertComponentDoesNotExist = new AssertComponentDoesNotExist(new ThrowHandler());
            _assertArrayNotEmpty = new AssertArrayNotEmpty(new ThrowHandler());
            _assertNotNull = new AssertNotNull(new ThrowHandler());
        }

        public ComponentMap(AssertComponentFound assertComponentFound, AssertComponentDoesNotExist assertComponentDoesNotExist, AssertArrayNotEmpty assertArrayNotEmpty, AssertNotNull assertNotNull)
        {
            _assertComponentFound = assertComponentFound;
            _assertComponentDoesNotExist = assertComponentDoesNotExist;
            _assertArrayNotEmpty = assertArrayNotEmpty;
            _assertNotNull = assertNotNull;
        }
        
        public void Add(params IComponent[] components)
        {
           _assertArrayNotEmpty.Handle(components.Length > 0);
            
            foreach (IComponent component in components)
            {
                _assertNotNull.AssertObjectNotNull(component);
                _assertComponentDoesNotExist.Handle(_components.ContainsKey(component.GetType()), component.GetType());
                _components.Add(component.GetType(), component);
            }
        }

        public void Remove<T>()
        {
            _assertComponentFound.Handle(_components.ContainsKey(typeof(T)), typeof(T));
            _components.Remove(typeof(T));
        }

        public IComponent Get<T>()
        {
            _assertComponentFound.Handle(_components.ContainsKey(typeof(T)), typeof(T));
            return _components[typeof(T)];
        }

        public bool Contains<T>()
        {
            return _components.ContainsKey(typeof(T));
        }

        public bool Contains(IComponent component)
        {
            return _components.ContainsKey(component.GetType());
        }
    }
}
