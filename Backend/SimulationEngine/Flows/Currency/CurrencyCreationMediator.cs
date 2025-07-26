using IdelPog.Common.Enums;
using IdelPog.Common.Repository;
using IdelPog.Messaging.Dispatch;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.SimulationEngine.Currency.Factories;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyCreationMediator : ICurrencyCreationMediator
    {
        private readonly IStateRepository<CurrencyType, Currency> _currencyRepository;
        private readonly IDispatchMany<CurrencyCreationDTO> _currencyCreationDTODispatcher;
        private readonly ICurrencyCreationDTOFactory _currencyCreationDTOFactory;
        private readonly IAssertNotNull _assertNotNull;
        private readonly IAssertCollectionNotEmpty _assertCollectionNotEmpty;
        private readonly IAssertNonDuplicate _assertNonDuplicate;
        private readonly INumberAssertion _numberAssertion;

        public CurrencyCreationMediator(IStateRepository<CurrencyType, Currency> currencyRepository,
            IDispatchMany<CurrencyCreationDTO> currencyCreationDTODispatcher, ICurrencyCreationDTOFactory currencyCreationDTOFactory,
            IAssertNotNull assertNotNull, IAssertCollectionNotEmpty assertCollectionNotEmpty, IAssertNonDuplicate assertNonDuplicate,
            INumberAssertion numberAssertion)
        {
            _currencyRepository = currencyRepository;
            _currencyCreationDTODispatcher = currencyCreationDTODispatcher;
            _currencyCreationDTOFactory = currencyCreationDTOFactory;
            _assertNotNull = assertNotNull;
            _assertCollectionNotEmpty = assertCollectionNotEmpty;
            _assertNonDuplicate = assertNonDuplicate;
            _numberAssertion = numberAssertion;
        }

        public void CreateCurrency(IReadOnlyList<CurrencyCreation> currencies)
        {
            _assertNotNull.AssertObjectNotNull(currencies);
            _assertCollectionNotEmpty.Handle(currencies);

            Dictionary<CurrencyType, Currency> createdCurrencies = new(currencies.Count);
            foreach (CurrencyCreation currencyCreation in currencies)
            {
                _numberAssertion.AssertNonNegative(currencyCreation.StartingAmount);
                _assertNonDuplicate.AssertContains(currencyCreation, () => _currencyRepository.Contains(currencyCreation.CurrencyType));

                // TODO: currency factory
                Currency currency = new(currencyCreation.CurrencyType, currencyCreation.StartingAmount);

                _assertNonDuplicate.AssertContains(currencyCreation, () => !createdCurrencies.TryAdd(currency.CurrencyType, currency));
            }

            foreach (KeyValuePair<CurrencyType, Currency> keyValuePair in createdCurrencies)
            {
                _currencyRepository.Add(keyValuePair.Key, keyValuePair.Value);
            }

            _currencyCreationDTODispatcher.Dispatch(_currencyCreationDTOFactory.CreateFrom(currencies));
        }
    }
}