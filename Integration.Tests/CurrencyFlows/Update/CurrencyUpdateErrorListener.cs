using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.DTO;

namespace Integration.Tests.CurrencyFlows.Update
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
            
            Console.WriteLine(currencyUpdateErrorDTO.ErrorDetails.ErrorMessage);
            Console.WriteLine(currencyUpdateErrorDTO.ErrorDetails.Exception);
        }
    }
}