using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Listeners
{
    public class CurrencyTradeListener(ICurrencyController currencyController) : IBufferListener<CurrencyTrade>
    {
        public Type ListenerType { get; } = typeof(CurrencyTrade);
        
        public void Handle(IReadOnlyList<CurrencyTrade> buffer)
        {
            currencyController.UpdateCurrency(buffer);
        }
    }
}