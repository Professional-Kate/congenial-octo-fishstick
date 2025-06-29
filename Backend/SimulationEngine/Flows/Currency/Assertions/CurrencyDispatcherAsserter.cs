using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public class CurrencyDispatcherAsserter(AssertTradesAreValid assertTradesAreValid, AssertNotNull assertNotNull, AssertCollectionNotEmpty assertCollectionNotEmpty) : ICurrencyDispatcherAsserter
    {
        public void AssertTradeCollection(IReadOnlyList<CurrencyUpdate> trades)
        {
            assertNotNull.AssertObjectNotNull(trades);
            assertCollectionNotEmpty.Handle(trades);
            assertTradesAreValid.Handle(trades);
        }
    }
}