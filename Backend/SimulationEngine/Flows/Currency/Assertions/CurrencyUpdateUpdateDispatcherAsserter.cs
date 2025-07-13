using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public class CurrencyUpdateUpdateDispatcherAsserter(IAssertTradesAreValid assertTradesAreValid, IAssertNotNull assertNotNull, IAssertCollectionNotEmpty assertCollectionNotEmpty) : ICurrencyUpdateDispatcherAsserter
    {
        public void AssertTradeCollection(IReadOnlyList<CurrencyUpdate> trades)
        {
            assertNotNull.AssertObjectNotNull(trades);
            assertCollectionNotEmpty.Handle(trades);
            assertTradesAreValid.Handle(trades);
        }
    }
}