using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Integration.Tests
{
    internal sealed class ManagedResponseListener<TResponse> : IBufferListener<TResponse> where TResponse : struct
    {
        public Type ListenerType => typeof(TResponse);
        public bool WasCalled { get; private set; }
        public TResponse[] Responses { get; private set; } = null!;

        public void Handle(IReadOnlyList<TResponse> buffer)
        {
            WasCalled = true;
            Responses = buffer.ToArray();
        }

        internal void AssertWasCalled(bool wasCalled)
        { 
            Assert.That(WasCalled, Is.EqualTo(wasCalled));
        }

        internal void AssertResponseLength(int length)
        {
            Assert.That(Responses, Has.Length.EqualTo(length));
        }
    }
}