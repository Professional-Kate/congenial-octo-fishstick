using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.DTO;

namespace Integration.Tests.CurrencyFlows.Create
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