using IdelPog.Staging.Messaging;

namespace IdelPog.Staging.Tests.Messaging
{
    internal class TestListener<T> : IBufferListener<T>
    {
        public Type ListenerType => typeof(T);

        public bool WasCalled { get; private set; }
        public IReadOnlyList<T> BufferData { get; private set; }
        public int AmountCalled { get; private set; }
        
        public void Handle(IReadOnlyList<T> buffer)
        {
            WasCalled = true;
            BufferData = buffer;
            AmountCalled++;
        }
        
        public void ResetWasCalled() => WasCalled = false;
        
        public void ResetAmountCalled() => AmountCalled = 0;
    }
}