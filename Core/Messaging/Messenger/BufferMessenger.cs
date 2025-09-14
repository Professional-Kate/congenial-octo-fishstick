using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Listener;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Core.Messaging.Messenger
{
    public sealed class BufferMessenger : IBufferMessenger, IBufferDispatcher
    {
        private readonly Dictionary<Type, List<IListener>> _listeners = new();
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly IListenerAssertion _listenerAssertion;
        
        public BufferMessenger(IObjectNullAssertion objectNullAssertion, IListenerAssertion listenerAssertion)
        {
            _objectNullAssertion = objectNullAssertion;
            _listenerAssertion = listenerAssertion;
        }

        public void Subscribe(IListener listener)
        {
            _objectNullAssertion.AssertNotNull(listener, nameof(listener));

            if (_listeners.TryGetValue(listener.ListenerType, out List<IListener>? listeners) == false)
            {
                listeners = [];
                _listeners.Add(listener.ListenerType, listeners);
            }

            listeners.Add(listener);
        }

        public void Unsubscribe(IListener listener)
        {
            _objectNullAssertion.AssertNotNull(listener, nameof(listener));

            bool contains = _listeners.TryGetValue(listener.ListenerType, out List<IListener>? listeners);
            _listenerAssertion.AssertListenerFound(contains, listener);

            listeners!.Remove(listener);
        }

        public void DispatchMessage<T>(IReadOnlyList<T> buffer) where T : struct
        {
            _objectNullAssertion.AssertNotNull(buffer, nameof(buffer));

            Type type = typeof(T);

            if (_listeners.TryGetValue(type, out List<IListener>? listeners) == false)
            {
                // If the type doesn't exist then we just want to return, otherwise this would throw an exception
                return;
            }

            foreach (IListener listener in listeners)
            {
                try
                {
                    if (listener is IBufferListener<T> bufferListener)
                    {
                        bufferListener.Handle(buffer);
                    }

                    if (buffer.Count != 1)
                    {
                        continue;
                    }

                    if (listener is ISingleListener<T> singleListener)
                    {
                        singleListener.Handle(buffer[0]);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}