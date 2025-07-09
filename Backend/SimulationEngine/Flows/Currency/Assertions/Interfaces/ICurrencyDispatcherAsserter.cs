using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public interface ICurrencyDispatcherAsserter
    {
        public void AssertTradeCollection(IReadOnlyList<CurrencyUpdate> trades);
    }
}