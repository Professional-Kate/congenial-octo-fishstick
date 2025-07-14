using IdelPog.Common.Repository;
using IdelPog.Messaging.Dispatch;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Dispatchers;
using IdelPog.SimulationEngine.Structures;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.SimulationEngine.Currency
{
    /// <inheritdoc cref="ICurrencyUpdateMediator"/>
    public class CurrencyUpdateMediator : ICurrencyUpdateMediator
    {
        private readonly ICurrencyService _currencyService;
        private readonly IStateRepository<CurrencyType, Currency> _currencyRepository;
        private readonly IDispatchMany<CurrencyUpdate> _currencyUpdateDispatcher;
        private readonly ICurrencyUpdateSummarizer _currencyUpdateSummarizer;
        private readonly IAssertPositive _assertPositive;
        private readonly IAssertCollectionNotEmpty _assertCollectionNotEmpty;
        private readonly IAssertFound _assertFound;
        private readonly IAssertNotNull _assertNotNull;
        
        public CurrencyUpdateMediator(
            IStateRepository<CurrencyType, Currency> stateRepository, 
            ICurrencyService currencyService, IDispatchMany<CurrencyUpdate> currencyUpdateDispatcher, ICurrencyUpdateSummarizer currencyUpdateSummarizer, 
            IAssertPositive assertPositive, IAssertCollectionNotEmpty assertCollectionNotEmpty, IAssertFound assertFound,  IAssertNotNull assertNotNull)
        {
            _currencyService = currencyService;
            _currencyRepository = stateRepository;
            _currencyUpdateDispatcher = currencyUpdateDispatcher;
            _currencyUpdateSummarizer = currencyUpdateSummarizer;
            _assertPositive = assertPositive;
            _assertCollectionNotEmpty = assertCollectionNotEmpty;
            _assertFound = assertFound;
            _assertNotNull = assertNotNull;
        }
        
        public void ProcessCurrencyUpdate(IReadOnlyList<CurrencyUpdate> updates)
        {
            _assertNotNull.AssertObjectNotNull(updates);
            AssertUpdates(updates);
            
            CurrencyUpdate[] summarizedUpdates = _currencyUpdateSummarizer.GetSummary(updates);
            _assertCollectionNotEmpty.Handle(summarizedUpdates);
            
            AllCurrenciesExist(summarizedUpdates);
            List<Currency> currencies = GetAllCurrencies(summarizedUpdates);
            UpdateCurrencies(MapUpdates(summarizedUpdates, currencies));
            
            _currencyUpdateDispatcher.Dispatch(summarizedUpdates);
        }

        private void AssertUpdates(IReadOnlyList<CurrencyUpdate> updates)
        {
            _assertCollectionNotEmpty.Handle(updates);
            _assertPositive.AssertNumberIsPositive<CurrencyUpdate>(updates.Select(entry => entry.Amount).ToArray());
        }

        private void AllCurrenciesExist(IReadOnlyList<CurrencyUpdate> trades)
        {
            foreach (CurrencyUpdate currencyTrade in trades)
            {
                _assertFound.AssertItemIsFound(currencyTrade.CurrencyType,() => _currencyRepository.Contains(currencyTrade.CurrencyType));
            }
        }

        private List<Currency> GetAllCurrencies(CurrencyUpdate[] updates)
        {
            List<Currency> currencies = new(updates.Length);
            foreach (CurrencyUpdate currencyUpdate in updates)
            {
                currencies.Add(_currencyRepository.Get(currencyUpdate.CurrencyType));
            }

            return currencies;
        }

        private static Dictionary<Currency, CurrencyUpdate> MapUpdates(CurrencyUpdate[] updates, List<Currency> currencies)
        {
            Dictionary<Currency, CurrencyUpdate> map = new();
            
            foreach (CurrencyUpdate currencyUpdate in updates)
            {
                Currency currency = currencies.Find(currency => currency.CurrencyType == currencyUpdate.CurrencyType)!;
                map.Add(currency, currencyUpdate);
            }

            return map;
        }

        private void UpdateCurrencies(Dictionary<Currency, CurrencyUpdate> mappedUpdates)
        {
            foreach ((Currency currency, CurrencyUpdate currencyUpdate) in mappedUpdates)
            {
                switch (currencyUpdate.Action)
                {
                    case ActionType.ADD:
                        _currencyService.AddAmount(currency, currencyUpdate.Amount);
                        break;
                    case ActionType.REMOVE:
                        _currencyService.RemoveAmount(currency, currencyUpdate.Amount);
                        break;
                }
                
                _currencyRepository.Update(currency.CurrencyType, currency);
            }
        }
    }
}