using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Currency.Contracts.Command;
using IdelPog.Currency.Contracts.Response;
using IdelPog.Currency.Factory.Interface;

namespace IdelPog.Currency.Mediator
{
    public sealed class CurrencyCreationMediator : IBatchMediator<CurrencyCreation>
    {
        private readonly IStateRepository<CurrencyType, Contracts.Currency> _currencyRepository;
        private readonly IDispatchMany<CurrencyCreationResponse> _responseDispatcher;
        private readonly ICurrencyCreationResponseFactory _currencyCreationResponseFactory;
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;

        public CurrencyCreationMediator(IStateRepository<CurrencyType, Contracts.Currency> currencyRepository,
            IDispatchMany<CurrencyCreationResponse> responseDispatcher, ICurrencyCreationResponseFactory currencyCreationResponseFactory,
            IObjectNullAssertion objectNullAssertion, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion)
        {
            _currencyRepository = currencyRepository;
            _responseDispatcher = responseDispatcher;
            _currencyCreationResponseFactory = currencyCreationResponseFactory;
            _objectNullAssertion = objectNullAssertion;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
        }

        public void HandleMessages(IReadOnlyList<CurrencyCreation> currencyCreations)
        {
            _objectNullAssertion.AssertNotNull(currencyCreations, nameof(currencyCreations));
            _collectionAssertion.AssertNotEmpty(currencyCreations);

            Contracts.Currency[] currencies = new Contracts.Currency[currencyCreations.Count];
            for (int i = 0; i < currencyCreations.Count; i++)
            {
                CurrencyCreation currencyCreation = currencyCreations[i];
                _uniqueAssertion.AssertUnique(currencyCreation, _currencyRepository.Contains(currencyCreation.CurrencyType));

                Contracts.Currency currency = new(currencyCreation.CurrencyType, currencyCreation.StartingAmount);
                currencies[i] = currency;
            }
            
            foreach (Contracts.Currency currency in currencies)
            {
                _currencyRepository.Add(currency.CurrencyType, currency);
            }
            
            _responseDispatcher.Dispatch(_currencyCreationResponseFactory.CreateFrom(currencyCreations));
        }
    }
}