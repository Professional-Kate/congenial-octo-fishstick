using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.Responses;

namespace Integration.Tests.CurrencyCommands.Create
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