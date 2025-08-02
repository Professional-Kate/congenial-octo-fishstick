using IdelPog.Common.Commands;
using IdelPog.SimulationEngine.Currency.Responses;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyUpdateResponseFactory : ICurrencyUpdateResponseFactory
    {
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public CurrencyUpdateResponseFactory(IObjectNullAssertion objectNullAssertion, ICollectionAssertion collectionAssertion)
        {
            _objectNullAssertion = objectNullAssertion;
            _collectionAssertion = collectionAssertion;
        }

        public CurrencyUpdateResponse CreateFrom(IReadOnlyList<CurrencyUpdate> trades)
        {
            _objectNullAssertion.AssertNotNull(trades, nameof(trades));
            _collectionAssertion.AssertNotEmpty(trades);

            return Create(trades.ToArray());
        }
        
        public CurrencyUpdateResponse CreateFrom(CurrencyUpdate trade)
        {
            _objectNullAssertion.AssertNotNull(trade, nameof(trade));

            return Create([trade]);
        }

        private static CurrencyUpdateResponse Create(CurrencyUpdate[] trades)
        {
            return new CurrencyUpdateResponse
            {
                CurrencyUpdates = trades
            };
        }
    }
}