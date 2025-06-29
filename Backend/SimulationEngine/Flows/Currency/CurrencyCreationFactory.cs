using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyCreationFactory : ICurrencyCreationFactory
    {
        private readonly IAssertNotNull _assertNotNull;
        private readonly IAssertCollectionNotEmpty _assertCollectionNotEmpty;

        public CurrencyCreationFactory(IAssertNotNull assertNotNull, IAssertCollectionNotEmpty assertCollectionNotEmpty)
        {
            _assertNotNull = assertNotNull;
            _assertCollectionNotEmpty = assertCollectionNotEmpty;
        }
        
        public CurrencyCreationDTO[] CreateFrom(IReadOnlyList<CurrencyCreation> trades)
        {
            _assertNotNull.AssertObjectNotNull(trades);
            _assertCollectionNotEmpty.Handle(trades);
            
            List<CurrencyCreationDTO> result = new(trades.Count);

            foreach (CurrencyCreation currencyCreation in trades)
            {
                result.Add(new CurrencyCreationDTO
                {
                    Currency = currencyCreation.CurrencyType,
                    Amount = currencyCreation.StartingAmount
                });
            }
            
            return result.ToArray();
        }
    }
}