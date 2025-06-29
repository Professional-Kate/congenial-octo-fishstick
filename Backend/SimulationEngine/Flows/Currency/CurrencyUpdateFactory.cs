using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyUpdateFactory(AssertNotNull assertNotNull, AssertCollectionNotEmpty assertCollectionNotEmpty) : ICurrencyUpdateFactory
    {
        public CurrencyUpdateDTO[] CreateFrom(IReadOnlyList<CurrencyTrade> trades)
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
            
            return result.ToArray();
        }
    }
}