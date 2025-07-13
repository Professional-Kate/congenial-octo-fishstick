using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public interface ICurrencyUpdateDispatcherAsserter
    {
        public void AssertTradeCollection(IReadOnlyList<CurrencyUpdate> trades);
    }
}