using IdelPog.Common.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyUpdateDTOFactory : ICurrencyUpdateDTOFactory
    {
        private readonly IAssertNotNull _assertNotNull;
        private readonly IAssertCollectionNotEmpty _assertCollectionNotEmpty;

        public CurrencyUpdateDTOFactory(IAssertNotNull assertNotNull, IAssertCollectionNotEmpty assertCollectionNotEmpty)
        {
            _assertNotNull = assertNotNull;
            _assertCollectionNotEmpty = assertCollectionNotEmpty;
        }
        
        public CurrencyUpdateDTO[] CreateFrom(IReadOnlyList<CurrencyUpdate> trades)
        {
            _assertNotNull.AssertObjectNotNull(trades);
            _assertCollectionNotEmpty.Handle(trades);
            
            List<CurrencyUpdateDTO> result = new(trades.Count);

            foreach (CurrencyUpdate currencyTrade in trades)
            {
                result.Add(new CurrencyUpdateDTO
                {
                    Action = currencyTrade.Action,
                    CurrencyType = currencyTrade.CurrencyType,
                    Amount = currencyTrade.Amount
                });
            }
            
            return result.ToArray();
        }
    }
}