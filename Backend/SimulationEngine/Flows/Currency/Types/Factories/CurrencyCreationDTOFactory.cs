using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyCreationDTOFactory : ICurrencyCreationDTOFactory
    {
        private readonly IAssertNotNull _assertNotNull;
        private readonly IAssertCollectionNotEmpty _assertCollectionNotEmpty;

        public CurrencyCreationDTOFactory(IAssertNotNull assertNotNull, IAssertCollectionNotEmpty assertCollectionNotEmpty)
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
                    CurrencyType = currencyCreation.CurrencyType,
                    Amount = currencyCreation.StartingAmount
                });
            }

            return result.ToArray();
        }
    }
}