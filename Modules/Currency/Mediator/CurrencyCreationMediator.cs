using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Currency.Factory.Interface;

namespace IdelPog.Currency.Mediator
{
    public class CurrencyCreationMediator : IBatchMediator<CurrencyCreation>
    {
        private readonly IStateRepository<CurrencyType, Contracts.Currency> _currencyRepository;
        private readonly IDispatchOne<CurrencyCreationResponse> _currencyCreationDTODispatcher;
        private readonly ICurrencyCreationResponseFactory _currencyCreationResponseFactory;
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;

        public CurrencyCreationMediator(IStateRepository<CurrencyType, Contracts.Currency> currencyRepository,
            IDispatchOne<CurrencyCreationResponse> currencyCreationDTODispatcher, ICurrencyCreationResponseFactory currencyCreationResponseFactory,
            IObjectNullAssertion objectNullAssertion, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion)
        {
            _currencyRepository = currencyRepository;
            _currencyCreationDTODispatcher = currencyCreationDTODispatcher;
            _currencyCreationResponseFactory = currencyCreationResponseFactory;
            _objectNullAssertion = objectNullAssertion;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
        }

        public void HandleMessages(IReadOnlyList<CurrencyCreation> currencyCreations)
        {
            _objectNullAssertion.AssertNotNull(currencyCreations, nameof(currencyCreations));
            _collectionAssertion.AssertNotEmpty(currencyCreations);

            Dictionary<CurrencyType, Contracts.Currency> createdCurrencies = new(currencyCreations.Count);
            foreach (CurrencyCreation currencyCreation in currencyCreations)
            {
                _uniqueAssertion.AssertUnique(currencyCreation, _currencyRepository.Contains(currencyCreation.CurrencyType));

                // TODO: currency factory
                Contracts.Currency currency = new(currencyCreation.CurrencyType, currencyCreation.StartingAmount);

                _uniqueAssertion.AssertUnique(currencyCreation, !createdCurrencies.TryAdd(currency.CurrencyType, currency));
            }

            foreach (KeyValuePair<CurrencyType, Contracts.Currency> keyValuePair in createdCurrencies)
            {
                _currencyRepository.Add(keyValuePair.Key, keyValuePair.Value);
            }

            _currencyCreationDTODispatcher.Dispatch(_currencyCreationResponseFactory.CreateFrom(currencyCreations));
        }
    }
}