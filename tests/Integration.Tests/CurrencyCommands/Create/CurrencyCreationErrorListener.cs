using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.Responses;

namespace Integration.Tests.CurrencyCommands.Create
{
    internal class CurrencyCreationErrorListener : ISingleListener<CurrencyCreationError>
    {
        public Type ListenerType { get; } = typeof(CurrencyCreationError);
        public CurrencyCreationError CurrencyUpdateError { get; private set; }
        public bool WasCalled { get; private set; }

        public void Handle(CurrencyCreationError item)
        {
            CurrencyUpdateError = item;
            WasCalled = true;
        }
    }
}