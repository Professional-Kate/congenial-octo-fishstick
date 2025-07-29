using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyCreationController : IBatchController<CurrencyCreation>
    {
        private readonly ICurrencyCreationMediator _currencyCreationMediator;
        
        public CurrencyCreationController(ICurrencyCreationMediator currencyCreationMediator)
        {
            _currencyCreationMediator = currencyCreationMediator;
        }
        
        public void HandleMessages(IReadOnlyList<CurrencyCreation> messages)
        {
            _currencyCreationMediator.CreateCurrency(messages);
        }
    }
}