using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.DTO;

namespace Integration.Tests.CurrencyCommands.Update
{
    internal class CurrencyUpdateErrorListener : ISingleListener<CurrencyUpdateErrorDTO>
    {
        public Type ListenerType { get; } = typeof(CurrencyUpdateErrorDTO);
        public CurrencyUpdateErrorDTO CurrencyUpdateErrorDTO { get; private set; }
        public bool WasCalled { get; private set; }
        
        public void Handle(CurrencyUpdateErrorDTO currencyUpdateErrorDTO)
        {
            WasCalled = true;
            CurrencyUpdateErrorDTO = currencyUpdateErrorDTO;
        }
    }
}