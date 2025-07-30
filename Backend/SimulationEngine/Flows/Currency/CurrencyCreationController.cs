using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyCreationController : IBatchController<CurrencyCreation>
    {
        private readonly IBatchMediator<CurrencyCreation> _currencyCreationMediator;
        
        public CurrencyCreationController(IBatchMediator<CurrencyCreation> currencyCreationMediator)
        {
            _currencyCreationMediator = currencyCreationMediator;
        }
        
        public void HandleMessages(IReadOnlyList<CurrencyCreation> messages)
        {
            _currencyCreationMediator.HandleMessages(messages);
        }
    }
}