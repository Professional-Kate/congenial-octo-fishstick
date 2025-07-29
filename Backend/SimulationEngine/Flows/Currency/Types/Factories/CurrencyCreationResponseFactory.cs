using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Responses;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyCreationResponseFactory : ICurrencyCreationResponseFactory
    {
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public CurrencyCreationResponseFactory(IObjectNullAssertion objectNullAssertion, ICollectionAssertion collectionAssertion)
        {
            _objectNullAssertion = objectNullAssertion;
            _collectionAssertion = collectionAssertion;
        }

        public CurrencyCreationResponse[] CreateFrom(IReadOnlyList<CurrencyCreation> trades)
        {
            _objectNullAssertion.AssertNotNull(trades, nameof(trades));
            _collectionAssertion.AssertNotEmpty(trades);

            List<CurrencyCreationResponse> result = new(trades.Count);

            foreach (CurrencyCreation currencyCreation in trades)
            {
                result.Add(new CurrencyCreationResponse
                {
                    CurrencyType = currencyCreation.CurrencyType,
                    Amount = currencyCreation.StartingAmount
                });
            }

            return result.ToArray();
        }

        public CurrencyCreationResponse CreateFrom(CurrencyCreation trade)
        {
            _objectNullAssertion.AssertNotNull(trade, nameof(trade));

            return new CurrencyCreationResponse
            {
                Amount = trade.StartingAmount,
                CurrencyType = trade.CurrencyType
            };
        }
    }
}