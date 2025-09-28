using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.ECS.Assertion;
using IdelPog.ECS.Component;

namespace IdelPog.Progression.Runtime.Component
{
    public readonly struct QueueComponentStore<T> : IComponent<QueueComponentStore<T>> where T : struct, IComponent<T>
    {
        private readonly Queue<T> _components;
        private readonly IHandler _handler;

        public QueueComponentStore(T[] components, IHandler handler)
        {
            _handler = handler;
            ComponentArrayAssertion componentArrayAssertion = new(_handler);
            componentArrayAssertion.AssertHasElements(components.ToArray());
            
            _components = new Queue<T>(components);
        }

        public T Peek()
        { 
            return _components.Peek();
        }

        public bool TryDequeue(out T component)
        {
            if (_components.Count <= 0)
            {
                component = default;
                return false;
            }

            component = _components.Dequeue();
            return true;
        }

        public T[] ToArray()
        {
            return _components.ToArray();
        }

        public QueueComponentStore<T> DeepClone()
        {
            return new QueueComponentStore<T>(_components.ToArray(), _handler);
        }
    }
}