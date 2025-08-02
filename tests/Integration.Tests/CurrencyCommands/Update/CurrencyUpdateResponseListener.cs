using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.Responses;

namespace Integration.Tests.CurrencyCommands.Update
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