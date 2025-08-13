using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.CurrencyCommands.Update
{
    internal class CurrencyUpdateResponseListener : ISingleListener<CurrencyUpdateResponse>
    {
        public Type ListenerType { get; } = typeof(CurrencyUpdateResponse);
        public CurrencyUpdateResponse Item { get; private set; }
        public bool WasCalled { get; private set; }

        public void Handle(CurrencyUpdateResponse message)
        {
            WasCalled = true;
            Item = message;
        }
    }
}