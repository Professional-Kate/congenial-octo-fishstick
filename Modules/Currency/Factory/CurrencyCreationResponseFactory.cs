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

        public IReadOnlyList<CurrencyCreationResponse> CreateFrom(IReadOnlyList<CurrencyCreation> currencyCreations)
        {
            _objectNullAssertion.AssertNotNull(currencyCreations, nameof(currencyCreations));
            _collectionAssertion.AssertNotEmpty(currencyCreations);

            return Create(currencyCreations);
        }

        private static CurrencyCreationResponse[] Create(IReadOnlyList<CurrencyCreation> currencyCreations)
        {
            CurrencyCreationResponse[] responses = new CurrencyCreationResponse[currencyCreations.Count];
            for (var i = 0; i < currencyCreations.Count; i++)
            {
                CurrencyCreation creation = currencyCreations[i];
                CurrencyCreationResponse currencyCreationResponse = new() { Amount = creation.StartingAmount, CurrencyType = creation.CurrencyType };
                responses[i] = currencyCreationResponse;
            }

            return responses;
        }
    }
}