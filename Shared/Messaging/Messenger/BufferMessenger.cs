using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Messaging.Listeners.Single;
using IdelPog.Validation.Assertions;

namespace IdelPog.Messaging.Messenger
{
    public class BufferMessenger(IObjectNullAssertion objectNullAssertion, IListenerAssertion listenerAssertion) : IBufferMessenger, IBufferDispatcher
    {
        private readonly Dictionary<Type, List<IListener>> _listeners = new();

        public void Subscribe(IListener listener)
        {
            objectNullAssertion.AssertNotNull(listener, nameof(listener));

            if (_listeners.TryGetValue(listener.ListenerType, out List<IListener>? listeners) == false)
            {
                listeners = [];
                _listeners.Add(listener.ListenerType, listeners);
            }

            listeners.Add(listener);
        }

        public void Unsubscribe(IListener listener)
        {
            objectNullAssertion.AssertNotNull(listener, nameof(listener));

            bool contains = _listeners.TryGetValue(listener.ListenerType, out List<IListener>? listeners);
            listenerAssertion.AssertListenerFound(contains, listener);

            listeners!.Remove(listener);
        }

        public void DispatchMessage<T>(IReadOnlyList<T> buffer)
        {
            objectNullAssertion.AssertNotNull(buffer, nameof(buffer));

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