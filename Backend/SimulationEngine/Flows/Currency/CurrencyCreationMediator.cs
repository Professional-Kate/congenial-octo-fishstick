using IdelPog.Common.Repository;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyCreationMediator : ICurrencyCreationMediator
    {
        private readonly IStateRepository<CurrencyType, Currency> _currencyRepository;
        private readonly ICurrencyCreationDispatcher _currencyCreationDispatcher;
        private readonly IAssertNotNull _assertNotNull;
        private readonly IAssertCollectionNotEmpty _assertCollectionNotEmpty;
        private readonly IAssertNonDuplicate _assertNonDuplicate;
        private readonly IAssertPositive _assertPositive;

        public CurrencyCreationMediator(IStateRepository<CurrencyType, Currency> currencyRepository,  ICurrencyCreationDispatcher currencyCreationDispatcher, IAssertNotNull assertNotNull, IAssertCollectionNotEmpty assertCollectionNotEmpty,  IAssertNonDuplicate assertNonDuplicate, IAssertPositive assertPositive)
        {
            _currencyRepository = currencyRepository;
            _currencyCreationDispatcher = currencyCreationDispatcher;
            _assertNotNull = assertNotNull;
            _assertCollectionNotEmpty = assertCollectionNotEmpty;
            _assertNonDuplicate = assertNonDuplicate;
            _assertPositive = assertPositive;
        }
        
        public void CreateCurrency(IReadOnlyList<CurrencyCreation> currencies)
        {
            _assertNotNull.AssertObjectNotNull(currencies);
            _assertCollectionNotEmpty.Handle(currencies);

            Dictionary<CurrencyType, Currency> createdCurrencies =  new(currencies.Count);
            foreach (CurrencyCreation currencyCreation in currencies)
            {
                _assertPositive.AssertNumberIsPositive(currencyCreation.StartingAmount);
                _assertNonDuplicate.AssertContains(currencyCreation, () => _currencyRepository.Contains(currencyCreation.CurrencyType));
                
                // TODO: currency factory
                Currency currency = new(currencyCreation.CurrencyType, currencyCreation.StartingAmount);
                
                _assertNonDuplicate.AssertContains(currencyCreation, () => !createdCurrencies.TryAdd(currency.CurrencyType, currency));
            }

            foreach (KeyValuePair<CurrencyType, Currency> keyValuePair in createdCurrencies)
            {
                _currencyRepository.Add(keyValuePair.Key, keyValuePair.Value);
            }
            
            _currencyCreationDispatcher.Dispatch(currencies);
        }
    }
}