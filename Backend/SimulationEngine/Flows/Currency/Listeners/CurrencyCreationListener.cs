using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Dispatchers;

namespace IdelPog.SimulationEngine.Currency.Listeners
{
    public class CurrencyCreationListener : IBufferListener<CurrencyCreation>
    {
        private readonly ICurrencyController _currencyController;
        private readonly ICurrencyCreationErrorDispatcher  _currencyCreationErrorDispatcher;

        public CurrencyCreationListener(ICurrencyController currencyController,  ICurrencyCreationErrorDispatcher currencyCreationErrorDispatcher)
        {
            _currencyController = currencyController;
            _currencyCreationErrorDispatcher = currencyCreationErrorDispatcher;
        }
        
        public Type ListenerType { get; } = typeof(CurrencyCreation);
        
        public void Handle(IReadOnlyList<CurrencyCreation> buffer)
        {
            try
            {
                _currencyController.CreateCurrency(buffer);
            }
            catch (Exception exception)
            {
                _currencyCreationErrorDispatcher.Dispatch(buffer, exception);
            }
        }
    }
}