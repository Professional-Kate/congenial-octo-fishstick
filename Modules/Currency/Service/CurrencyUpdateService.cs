using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Currency.Contracts.Response;
using IdelPog.Currency.Factory.Interface;
using IdelPog.Currency.Service.Interface;

namespace IdelPog.Currency.Service
{
    public sealed class CurrencyUpdateService : ICurrencyUpdateService
    {
        private readonly ICurrencyUpdateSummarizer _currencyUpdateSummarizer;
        private readonly ICurrencyService _currencyService;
        private readonly IStateRepository<CurrencyType, Contracts.Currency> _currencyRepository;
        private readonly ICurrencyUpdateResponseFactory _currencyUpdateResponseFactory;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IFoundAssertion _foundAssertion;

        public CurrencyUpdateService(ICurrencyService currencyService, IStateRepository<CurrencyType, Contracts.Currency> currencyRepository, ICollectionAssertion collectionAssertion, IFoundAssertion foundAssertion, ICurrencyUpdateResponseFactory currencyUpdateResponseFactory, ICurrencyUpdateSummarizer currencyUpdateSummarizer)
        {
            _currencyService = currencyService;
            _currencyRepository = currencyRepository;
            _collectionAssertion = collectionAssertion;
            _foundAssertion = foundAssertion;
            _currencyUpdateResponseFactory = currencyUpdateResponseFactory;
            _currencyUpdateSummarizer = currencyUpdateSummarizer;
        }

        public IReadOnlyList<CurrencyUpdateResponse> ApplyUpdates(IReadOnlyList<CurrencyUpdate> currencyUpdates)
        {
            _collectionAssertion.AssertHasElements(currencyUpdates);

            IReadOnlyList<CurrencyUpdate> summerizedUpdates = _currencyUpdateSummarizer.GetSummary(currencyUpdates);
            _collectionAssertion.AssertHasElements(summerizedUpdates);
            
            AllCurrenciesExist(summerizedUpdates);
            List<Contracts.Currency> currencies = GetAllCurrencies(summerizedUpdates);
            Dictionary<Contracts.Currency, CurrencyUpdate> mappedUpdates = MapUpdates(summerizedUpdates, currencies);
            UpdateCurrencies(mappedUpdates);

            return _currencyUpdateResponseFactory.CreateFrom(currencies);
        }
        
        private void AllCurrenciesExist(IReadOnlyList<CurrencyUpdate> trades)
        {
            foreach (CurrencyUpdate currencyTrade in trades)
            {
                _foundAssertion.AssertFound(currencyTrade.CurrencyType, _currencyRepository.Contains(currencyTrade.CurrencyType));
            }
        }
        
        private List<Contracts.Currency> GetAllCurrencies(IReadOnlyList<CurrencyUpdate> updates)
        {
            List<Contracts.Currency> currencies = new(updates.Count);
            foreach (CurrencyUpdate currencyUpdate in updates)
            {
                currencies.Add(_currencyRepository.Get(currencyUpdate.CurrencyType));
            }

            return currencies;
        }
        
        private static Dictionary<Contracts.Currency, CurrencyUpdate> MapUpdates(IReadOnlyList<CurrencyUpdate> updates, List<Contracts.Currency> currencies)
        {
            Dictionary<Contracts.Currency, CurrencyUpdate> map = new();

            foreach (CurrencyUpdate currencyUpdate in updates)
            {
                Contracts.Currency currency = currencies.Find(currency => currency.CurrencyType == currencyUpdate.CurrencyType)!;
                map.Add(currency, currencyUpdate);
            }

            return map;
        }
        
        private void UpdateCurrencies(Dictionary<Contracts.Currency, CurrencyUpdate> mappedUpdates)
        {
            foreach ((Contracts.Currency currency, CurrencyUpdate currencyUpdate) in mappedUpdates)
            {
                switch (currencyUpdate.ActionType)
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