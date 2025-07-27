using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyCreationDTOFactory : ICurrencyCreationDTOFactory
    {
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public CurrencyCreationDTOFactory(IObjectNullAssertion objectNullAssertion, ICollectionAssertion collectionAssertion)
        {
            _objectNullAssertion = objectNullAssertion;
            _collectionAssertion = collectionAssertion;
        }

        public CurrencyCreationDTO[] CreateFrom(IReadOnlyList<CurrencyCreation> trades)
        {
            _objectNullAssertion.AssertNotNull(trades, nameof(trades));
            _collectionAssertion.AssertNotEmpty(trades);

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

        public CurrencyCreationDTO CreateFrom(CurrencyCreation trade)
        {
            _objectNullAssertion.AssertNotNull(trade, nameof(trade));

            return new CurrencyCreationDTO
            {
                Amount = trade.StartingAmount,
                CurrencyType = trade.CurrencyType
            };
        }
    }
}