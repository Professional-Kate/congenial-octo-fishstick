using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Messaging;

namespace IdelPog.Messaging.Tests.Messaging
{
    public class SingleTestListener<T> : ISingleListener<T>
    {
        public Type ListenerType { get; } = typeof(T);
        public bool WasCalled { get; private set; }
        
        public void Handle(T item)
        {
            WasCalled = true;
        }

        public void ResetObject()
        {
            WasCalled = false;
        }
    }
}