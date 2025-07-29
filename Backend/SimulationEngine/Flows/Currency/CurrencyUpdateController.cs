using IdelPog.Common.Commands;
using IdelPog.Messaging.Listeners.Buffer;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyUpdateController : IBatchController<CurrencyUpdate>
    {
        private readonly ICurrencyUpdateMediator _currencyUpdateMediator;
        
        public CurrencyUpdateController(ICurrencyUpdateMediator currencyUpdateMediator)
        {
            _currencyUpdateMediator = currencyUpdateMediator;
        }
        
        public void HandleMessages(IReadOnlyList<CurrencyUpdate> messages)
        {
            _currencyUpdateMediator.ProcessCurrencyUpdate(messages);
        }
    }
}