using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Currency.Factory.Interface;

namespace IdelPog.Currency.Factory
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

        public CurrencyCreationResponse CreateFrom(IReadOnlyList<CurrencyCreation> currencyCreations)
        {
            _objectNullAssertion.AssertNotNull(currencyCreations, nameof(currencyCreations));
            _collectionAssertion.AssertNotEmpty(currencyCreations);

            return Create(currencyCreations.ToArray());
        }

        public CurrencyCreationResponse CreateFrom(CurrencyCreation currencyCreation)
        {
            _objectNullAssertion.AssertNotNull(currencyCreation, nameof(currencyCreation));

            return Create([currencyCreation]);
        }

        private CurrencyCreationResponse Create(CurrencyCreation[] currencyCreations)
        {
            return new CurrencyCreationResponse
            {
                CurrencyCreations = currencyCreations
            };
        }
    }
}