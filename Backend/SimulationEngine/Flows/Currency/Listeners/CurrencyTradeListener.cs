using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Listeners
{
    public class CurrencyTradeListener(ICurrencyController currencyController) : IBufferListener<CurrencyUpdate>
    {
        public Type ListenerType { get; } = typeof(CurrencyUpdate);
        
        public void Handle(IReadOnlyList<CurrencyUpdate> buffer)
        {
            currencyController.UpdateCurrency(buffer);
        }
    }
}