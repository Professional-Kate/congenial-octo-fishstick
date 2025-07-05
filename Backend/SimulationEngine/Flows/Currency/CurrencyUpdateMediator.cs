using IdelPog.Common.Repository;
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
        private readonly ICurrencyUpdateDispatcher _currencyUpdateDispatcher;
        private readonly ICurrencyUpdateSummarizer _currencyUpdateSummarizer;
        private readonly IAssertPositive _assertPositive;
        private readonly IAssertCollectionNotEmpty _assertCollectionNotEmpty;
        private readonly IAssertFound _assertFound;
        
        public CurrencyUpdateMediator(ICurrencyService currencyService, IStateRepository<CurrencyType, Currency> stateRepository, ICurrencyUpdateDispatcher currencyUpdateDispatcher, ICurrencyUpdateSummarizer currencyUpdateSummarizer, IAssertPositive assertPositive, IAssertCollectionNotEmpty assertCollectionNotEmpty, IAssertFound assertFound)
        {
            _currencyService = currencyService;
            _currencyRepository = stateRepository;
            _currencyUpdateDispatcher = currencyUpdateDispatcher;
            _currencyUpdateSummarizer = currencyUpdateSummarizer;
            _assertPositive = assertPositive;
            _assertCollectionNotEmpty = assertCollectionNotEmpty;
            _assertFound = assertFound;
        }
        
        public void ProcessCurrencyUpdate(IReadOnlyList<CurrencyUpdate> updates)
        {
            AssertUpdates(updates);
            
            CurrencyUpdate[] summarizedUpdates = _currencyUpdateSummarizer.GetSummary(updates);
            _assertCollectionNotEmpty.Handle(summarizedUpdates);
            
            AllCurrenciesExist(summarizedUpdates);
            List<Currency> currencies = GetAllCurrencies(summarizedUpdates);
            
            Dictionary<Currency, CurrencyUpdate> mappedUpdates = MapUpdates(summarizedUpdates, currencies);
            AssertCanRemoveCurrency(FilterRemoveUpdates(mappedUpdates));
            UpdateCurrencies(mappedUpdates);
            
            _currencyUpdateDispatcher.Dispatch(summarizedUpdates);
        }

        private void AssertUpdates(IReadOnlyList<CurrencyUpdate> updates)
        {
            _assertCollectionNotEmpty.Handle(updates);
            _assertPositive.AssertNumberIsPositive(updates.Select(entry => entry.Amount).ToArray());
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

        private static Dictionary<Currency, CurrencyUpdate> FilterRemoveUpdates(Dictionary<Currency, CurrencyUpdate> mappedUpdates)
        {
            Dictionary<Currency, CurrencyUpdate> removeUpdates = [];

            foreach ((Currency currency, CurrencyUpdate currencyUpdate) in mappedUpdates)
            {
                if (currencyUpdate.Action != ActionType.REMOVE)
                {
                    continue;
                }
                
                removeUpdates.Add(currency, currencyUpdate);
            }

            return removeUpdates;
        }

        private void AssertCanRemoveCurrency(Dictionary<Currency, CurrencyUpdate> mappedRemoveUpdates)
        {
            foreach ((Currency currency, CurrencyUpdate currencyUpdate) in mappedRemoveUpdates)
            {
                if (currencyUpdate.Action != ActionType.REMOVE)
                {
                    continue;
                }

                _assertPositive.AssertNumberIsPositive(currency.Amount - currencyUpdate.Amount);
            }
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