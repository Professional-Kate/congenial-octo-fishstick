using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Listeners
{
    public class CurrencyCreationListener(ICurrencyController currencyController) : IBufferListener<CurrencyCreation>
    {
        public Type ListenerType { get; } = typeof(CurrencyCreation);
        
        public void Handle(IReadOnlyList<CurrencyCreation> buffer)
        {
            currencyController.CreateCurrency(buffer);
        }
    }
}