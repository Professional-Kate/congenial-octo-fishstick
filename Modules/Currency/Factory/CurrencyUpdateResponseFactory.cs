using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Currency.Factory.Interface;

namespace IdelPog.Currency.Factory
{
    public sealed class CurrencyUpdateResponseFactory : ICurrencyUpdateResponseFactory
    {
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public CurrencyUpdateResponseFactory(IObjectNullAssertion objectNullAssertion, ICollectionAssertion collectionAssertion)
        {
            _objectNullAssertion = objectNullAssertion;
            _collectionAssertion = collectionAssertion;
        }

        public IReadOnlyList<CurrencyUpdateResponse> CreateFrom(IReadOnlyList<Contracts.Currency> currencies)
        {
            _objectNullAssertion.AssertNotNull(currencies, nameof(currencies));
            _collectionAssertion.AssertNotEmpty(currencies);

            return Create(currencies);
        }
        
        private static CurrencyUpdateResponse[] Create(IReadOnlyList<Contracts.Currency> currencies)
        {
            CurrencyUpdateResponse[] responses = new CurrencyUpdateResponse[currencies.Count];
            for (int i = 0; i < currencies.Count; i++)
            {
                Contracts.Currency currency = currencies[i];
                CurrencyUpdateResponse response = new() { CurrencyType = currency.CurrencyType, CurrencyAmount = currency.Amount };
                responses[i] = response;
            }

            return responses;
        }
    }
}