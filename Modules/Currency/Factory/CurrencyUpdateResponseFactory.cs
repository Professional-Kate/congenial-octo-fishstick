using IdelPog.Core.Contracts.Command;
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

        public IReadOnlyList<CurrencyUpdateResponse> CreateFrom(IReadOnlyList<CurrencyUpdate> trades)
        {
            _objectNullAssertion.AssertNotNull(trades, nameof(trades));
            _collectionAssertion.AssertNotEmpty(trades);

            return Create(trades);
        }
        
        private static CurrencyUpdateResponse[] Create(IReadOnlyList<CurrencyUpdate> trades)
        {
            CurrencyUpdateResponse[] responses = new CurrencyUpdateResponse[trades.Count];
            for (int i = 0; i < trades.Count; i++)
            {
                CurrencyUpdate currencyUpdate = trades[i];
                CurrencyUpdateResponse response = new() { CurrencyType = currencyUpdate.CurrencyType, ActionType = currencyUpdate.ActionType, Amount = currencyUpdate.Amount };
                responses[i] = response;
            }

            return responses;
        }
    }
}