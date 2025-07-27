using IdelPog.Messaging.Listeners;

namespace IdelPog.Messaging.Tests.Messaging
{
    public class SingleTestListener<T> : ISingleListener<T>
    {
        public Type ListenerType { get; } = typeof(T);
        public bool WasCalled { get; private set; }
        public T Data { get; private set; }

        public void Handle(T harvestNode)
        {
            WasCalled = true;
            Data = harvestNode;
        }

        public void ResetObject()
        {
            WasCalled = false;
        }
    }
}