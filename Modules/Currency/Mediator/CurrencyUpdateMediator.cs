using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Currency.Contracts.Response;
using IdelPog.Currency.Service.Interface;

namespace IdelPog.Currency.Mediator
{
    public sealed class CurrencyUpdateMediator : IBatchMediator<CurrencyUpdate>
    {
        private readonly ICurrencyUpdateService _currencyUpdateService;
        private readonly IDispatchMany<CurrencyUpdateResponse> _currencyUpdateDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;

        public CurrencyUpdateMediator(ICurrencyUpdateService currencyUpdateService, IDispatchMany<CurrencyUpdateResponse> currencyUpdateDispatcher, ICollectionAssertion collectionAssertion)
        {
            _currencyUpdateService = currencyUpdateService;
            _currencyUpdateDispatcher = currencyUpdateDispatcher;
            _collectionAssertion = collectionAssertion;
        }

        public void HandleMessages(IReadOnlyList<CurrencyUpdate> currencyUpdates)
        {
            _collectionAssertion.AssertHasElements(currencyUpdates);

            IReadOnlyList<CurrencyUpdateResponse> currencyUpdateResponses = _currencyUpdateService.ApplyUpdates(currencyUpdates);

            _currencyUpdateDispatcher.Dispatch(currencyUpdateResponses);
        }
    }
}