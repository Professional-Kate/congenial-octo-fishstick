using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Listeners.Buffer;

namespace IdelPog.Messaging.Tests.Messaging
{
    public class SingleTestListener<T> : ISingleListener<T>
    {
        public Type ListenerType { get; } = typeof(T);
        public bool WasCalled { get; private set; }
        public T Data { get; private set; }

        public void Handle(T item)
        {
            WasCalled = true;
            Data = item;
        }

        public void ResetObject()
        {
            WasCalled = false;
        }
    }
}