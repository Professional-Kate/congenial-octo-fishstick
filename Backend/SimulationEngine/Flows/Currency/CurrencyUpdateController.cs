using IdelPog.Common.Commands;
using IdelPog.Messaging.Listeners.Buffer;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyUpdateController : IBatchedController<CurrencyUpdate>
    {
        private readonly ICurrencyUpdateMediator _currencyUpdateMediator;
        
        public CurrencyUpdateController(ICurrencyUpdateMediator currencyUpdateMediator)
        {
            _currencyUpdateMediator = currencyUpdateMediator;
        }
        
        public void HandleMessages(IReadOnlyList<CurrencyUpdate> message)
        {
            _currencyUpdateMediator.ProcessCurrencyUpdate(message);
        }
    }
}