using IdelPog.Messaging.Listeners;

namespace IdelPog.SimulationEngine.Flows.Currency
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