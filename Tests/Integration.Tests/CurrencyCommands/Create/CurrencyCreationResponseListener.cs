using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.CurrencyCommands.Create
{
    internal class CurrencyCreationResponseListener : ISingleListener<CurrencyCreationResponse>
    {
        public Type ListenerType { get; } = typeof(CurrencyCreationResponse);
        public CurrencyCreationResponse Item { get; private set; }
        public bool WasCalled { get; private set; }
        
        public void Handle(CurrencyCreationResponse message)
        {
            Item = message;
            WasCalled = true;
        }
    }
}