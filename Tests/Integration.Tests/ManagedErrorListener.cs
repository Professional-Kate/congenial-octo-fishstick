using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests
{
    internal sealed class ManagedErrorListener<TError> : ISingleListener<TError> where TError : struct
    {
        public Type ListenerType => typeof(TError);
        public bool WasCalled { get; private set; }
        public TError Error { get; private set; }
        
        public void Handle(TError message)
        {
            WasCalled = true;
            Error = message;
        }
    }
}