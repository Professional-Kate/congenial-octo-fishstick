using IdelPog.Staging.Assertions;
using IdelPog.Validation.Assertions;

namespace IdelPog.Staging.Messaging
{
    public class BufferMessenger(IAssertNotNull assertNotNull, IAssertListenerFound assertListenerFound) : IBufferMessenger
    {
        private readonly Dictionary<Type, List<IListener>> _listeners = new();
        
        public void Subscribe<T>(IBufferListener<T> bufferListener)
        {
            assertNotNull.AssertObjectNotNull(bufferListener);
            
            Type type = typeof(T);

            if (_listeners.TryGetValue(type, out List<IListener>? listeners) == false)
            {
                listeners = [];
                _listeners.Add(type, listeners);
            }
            
            listeners!.Add(bufferListener);
        }

        public void Unsubscribe<T>(IBufferListener<T> bufferListener)
        {
            assertNotNull.AssertObjectNotNull(bufferListener);

            Type type = typeof(T);

            bool contains = _listeners.TryGetValue(type, out List<IListener>? listeners);
            assertListenerFound.AssertFound(bufferListener, contains);
            
            listeners!.Remove(bufferListener);
        }

        public void DispatchMessage<T>(IReadOnlyList<T> buffer)
        {
            assertNotNull.AssertObjectNotNull(buffer);

            Type type = typeof(T);

            if (_listeners.TryGetValue(type, out List<IListener>? listeners) == false)
            {
                return;
            }
            
            foreach (IBufferListener<T> bufferListener in listeners.OfType<IBufferListener<T>>())
            {
                bufferListener.Handle(buffer);
            }
        }
    }
}