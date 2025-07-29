using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.Responses;

namespace Integration.Tests.CurrencyCommands.Update
{
    internal class CurrencyUpdateErrorListener : ISingleListener<CurrencyUpdateError>
    {
        public Type ListenerType { get; } = typeof(CurrencyUpdateError);
        public CurrencyUpdateError CurrencyUpdateError { get; private set; }
        public bool WasCalled { get; private set; }

        public void Handle(CurrencyUpdateError currencyUpdateError)
        {
            WasCalled = true;
            CurrencyUpdateError = currencyUpdateError;
        }
    }
}