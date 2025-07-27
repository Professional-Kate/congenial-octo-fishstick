using IdelPog.Common.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyUpdateDTOFactory : ICurrencyUpdateDTOFactory
    {
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public CurrencyUpdateDTOFactory(IObjectNullAssertion objectNullAssertion, ICollectionAssertion collectionAssertion)
        {
            _objectNullAssertion = objectNullAssertion;
            _collectionAssertion = collectionAssertion;
        }

        public CurrencyUpdateDTO[] CreateFrom(IReadOnlyList<CurrencyUpdate> trades)
        {
            _objectNullAssertion.AssertNotNull(trades, nameof(trades));
            _collectionAssertion.AssertNotEmpty(trades);

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