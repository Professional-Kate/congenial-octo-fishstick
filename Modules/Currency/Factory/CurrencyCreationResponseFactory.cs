using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Currency.Factory.Interface;

namespace IdelPog.Currency.Factory
{
    public sealed class CurrencyCreationResponseFactory : ICurrencyCreationResponseFactory
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

        private static CurrencyCreationResponse Create(CurrencyCreation[] currencyCreations)
        {
            return new CurrencyCreationResponse
            {
                CurrencyCreations = currencyCreations
            };
        }
    }
}