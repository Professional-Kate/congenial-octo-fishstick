using IdelPog.Common.Enums;
using IdelPog.Common.Repository;
using IdelPog.Messaging.Dispatch;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Factories;
using IdelPog.SimulationEngine.Currency.Responses;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyCreationMediator : ICurrencyCreationMediator
    {
        private readonly IStateRepository<CurrencyType, Models.Currency> _currencyRepository;
        private readonly IDispatchMany<CurrencyCreationResponse> _currencyCreationDTODispatcher;
        private readonly ICurrencyCreationResponseFactory _currencyCreationResponseFactory;
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;

        public CurrencyCreationMediator(IStateRepository<CurrencyType, Models.Currency> currencyRepository,
            IDispatchMany<CurrencyCreationResponse> currencyCreationDTODispatcher, ICurrencyCreationResponseFactory currencyCreationResponseFactory,
            IObjectNullAssertion objectNullAssertion, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion)
        {
            _currencyRepository = currencyRepository;
            _currencyCreationDTODispatcher = currencyCreationDTODispatcher;
            _currencyCreationResponseFactory = currencyCreationResponseFactory;
            _objectNullAssertion = objectNullAssertion;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
        }

        public void CreateCurrency(IReadOnlyList<CurrencyCreation> currencies)
        {
            _objectNullAssertion.AssertNotNull(currencies, nameof(currencies));
            _collectionAssertion.AssertNotEmpty(currencies);

            Dictionary<CurrencyType, Models.Currency> createdCurrencies = new(currencies.Count);
            foreach (CurrencyCreation currencyCreation in currencies)
            {
                _uniqueAssertion.AssertUnique(currencyCreation, _currencyRepository.Contains(currencyCreation.CurrencyType));

                // TODO: currency factory
                Models.Currency currency = new(currencyCreation.CurrencyType, currencyCreation.StartingAmount);

                _uniqueAssertion.AssertUnique(currencyCreation, !createdCurrencies.TryAdd(currency.CurrencyType, currency));
            }

            foreach (KeyValuePair<CurrencyType, Models.Currency> keyValuePair in createdCurrencies)
            {
                _currencyRepository.Add(keyValuePair.Key, keyValuePair.Value);
            }

            _currencyCreationDTODispatcher.Dispatch(_currencyCreationResponseFactory.CreateFrom(currencies));
        }
    }
}