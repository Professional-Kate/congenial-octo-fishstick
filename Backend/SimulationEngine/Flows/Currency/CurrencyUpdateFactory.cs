using IdelPog.SimulationEngine.Flows.Currency.Assertions;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Flows.Currency
{
    public class CurrencyUpdateFactory(AssertNotNull assertNotNull, AssertCollectionNotEmpty assertCollectionNotEmpty) : ICurrencyUpdateFactory
    {
        public IReadOnlyList<CurrencyUpdateDTO> CreateFrom(IReadOnlyList<CurrencyTrade> trades)
        {
            assertNotNull.AssertObjectNotNull(trades);
            assertCollectionNotEmpty.Handle(trades);
            
            List<CurrencyUpdateDTO> result = new(trades.Count);

            foreach (CurrencyTrade currencyTrade in trades)
            {
                result.Add(new CurrencyUpdateDTO
                {
                    Action = currencyTrade.Action,
                    Currency = currencyTrade.Currency,
                    Amount = currencyTrade.Amount,
                });
            }
            
            return result;
        }
    }
}