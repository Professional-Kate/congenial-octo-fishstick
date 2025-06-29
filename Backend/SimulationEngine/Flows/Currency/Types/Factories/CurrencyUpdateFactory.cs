using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyUpdateFactory : ICurrencyUpdateFactory
    {
        private readonly IAssertNotNull _assertNotNull;
        private readonly IAssertCollectionNotEmpty _assertCollectionNotEmpty;

        public CurrencyUpdateFactory(IAssertNotNull assertNotNull, IAssertCollectionNotEmpty assertCollectionNotEmpty)
        {
            _assertNotNull = assertNotNull;
            _assertCollectionNotEmpty = assertCollectionNotEmpty;
        }
        
        public CurrencyUpdateDTO[] CreateFrom(IReadOnlyList<CurrencyTrade> trades)
        {
            _assertNotNull.AssertObjectNotNull(trades);
            _assertCollectionNotEmpty.Handle(trades);
            
            List<CurrencyUpdateDTO> result = new(trades.Count);

            foreach (CurrencyTrade currencyTrade in trades)
            {
                result.Add(new CurrencyUpdateDTO
                {
                    Action = currencyTrade.Action,
                    Currency = currencyTrade.Currency,
                    Amount = currencyTrade.Amount
                });
            }
            
            return result.ToArray();
        }
    }
}