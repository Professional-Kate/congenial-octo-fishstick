using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Flows.Currency.Assertions
{
    public class CurrencyDispatcherAsserter(AssertTradesAreValid assertTradesAreValid, AssertNotNull assertNotNull, AssertCollectionNotEmpty assertCollectionNotEmpty) : ICurrencyDispatcherAsserter
    {
        public void AssertTradeCollection(IReadOnlyList<CurrencyTrade> trades)
        {
            assertNotNull.AssertObjectNotNull(trades);
            assertCollectionNotEmpty.Handle(trades);
            assertTradesAreValid.Handle(trades);
        }
    }
}