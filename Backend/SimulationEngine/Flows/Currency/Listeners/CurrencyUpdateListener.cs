using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Dispatchers;

namespace IdelPog.SimulationEngine.Currency.Listeners
{
    public class CurrencyUpdateListener : IBufferListener<CurrencyUpdate>
    {
        private readonly ICurrencyController _currencyController;
        private readonly ICurrencyUpdateErrorDispatcher  _currencyUpdateErrorDispatcher;

        public CurrencyUpdateListener(ICurrencyController currencyController, ICurrencyUpdateErrorDispatcher currencyUpdateErrorDispatcher)
        {
            _currencyController = currencyController;
            _currencyUpdateErrorDispatcher = currencyUpdateErrorDispatcher;
        }
        
        public Type ListenerType { get; } = typeof(CurrencyUpdate);
        
        public void Handle(IReadOnlyList<CurrencyUpdate> buffer)
        {
            try
            {
                _currencyController.UpdateCurrency(buffer);
            }
            catch (Exception exception)
            {
                _currencyUpdateErrorDispatcher.Dispatch(buffer, exception);
            }
        }
    }
}