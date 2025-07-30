using IdelPog.Common.Commands;
using IdelPog.Messaging.Listeners.Buffer;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyUpdateController : IBatchController<CurrencyUpdate>
    {
        private readonly IBatchMediator<CurrencyUpdate> _currencyUpdateMediator;
        
        public CurrencyUpdateController(IBatchMediator<CurrencyUpdate> currencyUpdateMediator)
        {
            _currencyUpdateMediator = currencyUpdateMediator;
        }
        
        public void HandleMessages(IReadOnlyList<CurrencyUpdate> messages)
        {
            _currencyUpdateMediator.HandleMessages(messages);
        }
    }
}