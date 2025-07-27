using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.SimulationEngine.Currency.DTO;

namespace Integration.Tests.CurrencyCommands.Create
{
    internal class CurrencyCreationErrorListener : ISingleListener<CurrencyCreationErrorDTO>
    {
        public Type ListenerType { get; } = typeof(CurrencyCreationErrorDTO);
        public CurrencyCreationErrorDTO CurrencyUpdateErrorDTO { get; private set; }
        public bool WasCalled { get; private set; }

        public void Handle(CurrencyCreationErrorDTO item)
        {
            CurrencyUpdateErrorDTO = item;
            WasCalled = true;
        }
    }
}