using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Core.Tests.Messaging.Messaging
{
    public class SingleTestListener<T> : ISingleListener<T>
    {
        public Type ListenerType { get; } = typeof(T);
        public bool WasCalled { get; private set; }
        public T Data { get; private set; }

        public void Handle(T message)
        {
            WasCalled = true;
            Data = message;
        }

        public void ResetObject()
        {
            WasCalled = false;
        }
    }
}