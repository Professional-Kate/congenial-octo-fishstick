using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public interface ICurrencyUpdateDispatcherAsserter
    {
        public void AssertTradeCollection(IReadOnlyList<CurrencyUpdate> trades);
    }
}