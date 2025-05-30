using IdelPog.Staging.Messaging;

namespace IdelPog.Staging.Tests.Messaging
{
    internal class TestListener<T> : IBufferListener<T>
    {
        public Type ListenerType => typeof(T);

        public bool WasCalled { get; private set; }
        public IReadOnlyList<T> BufferData { get; private set; }
        public int AmountCalled { get; private set; }
        public bool ShouldThrowException { get; set; }
        
        public void Handle(IReadOnlyList<T> buffer)
        {
            WasCalled = true;
            BufferData = buffer;
            AmountCalled++;

            if (ShouldThrowException)
            {
                throw new Exception();
            }
        }

        public void ResetObject()
        {
            ResetWasCalled();
            ResetAmountCalled();
        }
        
        public void ResetWasCalled() => WasCalled = false;
        
        public void ResetAmountCalled() => AmountCalled = 0;
    }
}