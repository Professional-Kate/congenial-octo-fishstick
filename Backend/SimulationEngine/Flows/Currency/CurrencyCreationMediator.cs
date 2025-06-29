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

        public CurrencyCreationMediator(IStateRepository<CurrencyType, Currency> currencyRepository,  ICurrencyCreationDispatcher currencyCreationDispatcher, IAssertNotNull assertNotNull, IAssertCollectionNotEmpty assertCollectionNotEmpty)
        {
            _currencyRepository = currencyRepository;
            _currencyCreationDispatcher = currencyCreationDispatcher;
            _assertNotNull = assertNotNull;
            _assertCollectionNotEmpty = assertCollectionNotEmpty;
        }
        
        public void CreateCurrency(IReadOnlyList<CurrencyCreation> currencies)
        {
            _assertNotNull.AssertObjectNotNull(currencies);
            _assertCollectionNotEmpty.Handle(currencies);

            Dictionary<CurrencyType, Currency> createdCurrencies =  new(currencies.Count);
            foreach (CurrencyCreation currencyCreation in currencies)
            {
                // TODO: collection contains for both collections
                if (_currencyRepository.Contains(currencyCreation.CurrencyType))
                {
                    throw new Exception();
                }
                
                // TODO: currency factory
                Currency currency = new(currencyCreation.CurrencyType, currencyCreation.StartingAmount);
                
                if (createdCurrencies.TryAdd(currency.CurrencyType, currency) == false)
                {
                    throw new Exception();
                }
            }

            foreach (KeyValuePair<CurrencyType, Currency> keyValuePair in createdCurrencies)
            {
                _currencyRepository.Add(keyValuePair.Key, keyValuePair.Value);
            }
            
            _currencyCreationDispatcher.Dispatch(currencies);
        }
    }
}