using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Listeners
{
    public class CurrencyCreationListener : IBufferListener<CurrencyCreation>
    {
        private readonly ICurrencyController _currencyController;

        public CurrencyCreationListener(ICurrencyController currencyController)
        {
            _currencyController = currencyController;
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
                
            }
        }
    }
}