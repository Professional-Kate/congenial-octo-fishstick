using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Repository;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.SimulationEngine.Currency.Factories;
using IdelPog.SimulationEngine.Currency.Responses;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyUpdateMediator : IBatchMediator<CurrencyUpdate>
    {
        private readonly ICurrencyService _currencyService;
        private readonly IStateRepository<CurrencyType, Models.Currency> _currencyRepository;
        private readonly IDispatchOne<CurrencyUpdateResponse> _currencyUpdateDispatcher;
        private readonly ICurrencyUpdateSummarizer _currencyUpdateSummarizer;
        private readonly ICurrencyUpdateResponseFactory _currencyUpdateResponseFactory;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IFoundAssertion _foundAssertion;
        private readonly IObjectNullAssertion _objectNullAssertion;

        public CurrencyUpdateMediator(
            IStateRepository<CurrencyType, Models.Currency> stateRepository,
            ICurrencyService currencyService, IDispatchOne<CurrencyUpdateResponse> currencyUpdateDispatcher, ICurrencyUpdateSummarizer currencyUpdateSummarizer,
            ICurrencyUpdateResponseFactory currencyUpdateResponseFactory,
            ICollectionAssertion collectionAssertion, IFoundAssertion foundAssertion, IObjectNullAssertion objectNullAssertion)
        {
            _currencyService = currencyService;
            _currencyRepository = stateRepository;
            _currencyUpdateDispatcher = currencyUpdateDispatcher;
            _currencyUpdateSummarizer = currencyUpdateSummarizer;
            _currencyUpdateResponseFactory = currencyUpdateResponseFactory;
            _collectionAssertion = collectionAssertion;
            _foundAssertion = foundAssertion;
            _objectNullAssertion = objectNullAssertion;
        }

        
        public void HandleMessages(IReadOnlyList<CurrencyUpdate> currencyUpdates)
        {
            _objectNullAssertion.AssertNotNull(currencyUpdates, nameof(currencyUpdates));
            AssertUpdates(currencyUpdates);

            CurrencyUpdate[] summarizedUpdates = _currencyUpdateSummarizer.GetSummary(currencyUpdates);
            _collectionAssertion.AssertNotEmpty(summarizedUpdates);

            AllCurrenciesExist(summarizedUpdates);
            List<Models.Currency> currencies = GetAllCurrencies(summarizedUpdates);
            UpdateCurrencies(MapUpdates(summarizedUpdates, currencies));

            _currencyUpdateDispatcher.Dispatch(_currencyUpdateResponseFactory.CreateFrom(summarizedUpdates));
        }

        private void AssertUpdates(IReadOnlyList<CurrencyUpdate> updates)
        {
            _collectionAssertion.AssertNotEmpty(updates);
        }

        private void AllCurrenciesExist(IReadOnlyList<CurrencyUpdate> trades)
        {
            foreach (CurrencyUpdate currencyTrade in trades)
            {
                _foundAssertion.AssertFound(currencyTrade.CurrencyType, _currencyRepository.Contains(currencyTrade.CurrencyType));
            }
        }

        private List<Models.Currency> GetAllCurrencies(CurrencyUpdate[] updates)
        {
            List<Models.Currency> currencies = new(updates.Length);
            foreach (CurrencyUpdate currencyUpdate in updates)
            {
                currencies.Add(_currencyRepository.Get(currencyUpdate.CurrencyType));
            }

            return currencies;
        }

        private static Dictionary<Models.Currency, CurrencyUpdate> MapUpdates(CurrencyUpdate[] updates, List<Models.Currency> currencies)
        {
            Dictionary<Models.Currency, CurrencyUpdate> map = new();

            foreach (CurrencyUpdate currencyUpdate in updates)
            {
                Models.Currency currency = currencies.Find(currency => currency.CurrencyType == currencyUpdate.CurrencyType)!;
                map.Add(currency, currencyUpdate);
            }

            return map;
        }

        private void UpdateCurrencies(Dictionary<Models.Currency, CurrencyUpdate> mappedUpdates)
        {
            foreach ((Models.Currency currency, CurrencyUpdate currencyUpdate) in mappedUpdates)
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