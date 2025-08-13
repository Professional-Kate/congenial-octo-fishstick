using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Currency.Factory.Interface;
using IdelPog.Currency.Service.Interface;

namespace IdelPog.Currency.Mediator
{
    public class CurrencyUpdateMediator : IBatchMediator<CurrencyUpdate>
    {
        private readonly ICurrencyService _currencyService;
        private readonly IStateRepository<CurrencyType, Contracts.Currency> _currencyRepository;
        private readonly IDispatchOne<CurrencyUpdateResponse> _currencyUpdateDispatcher;
        private readonly ICurrencyUpdateSummarizer _currencyUpdateSummarizer;
        private readonly ICurrencyUpdateResponseFactory _currencyUpdateResponseFactory;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IFoundAssertion _foundAssertion;
        private readonly IObjectNullAssertion _objectNullAssertion;

        public CurrencyUpdateMediator(
            IStateRepository<CurrencyType, Contracts.Currency> stateRepository,
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
            List<Contracts.Currency> currencies = GetAllCurrencies(summarizedUpdates);
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

        private List<Contracts.Currency> GetAllCurrencies(CurrencyUpdate[] updates)
        {
            List<Contracts.Currency> currencies = new(updates.Length);
            foreach (CurrencyUpdate currencyUpdate in updates)
            {
                currencies.Add(_currencyRepository.Get(currencyUpdate.CurrencyType));
            }

            return currencies;
        }

        private static Dictionary<Contracts.Currency, CurrencyUpdate> MapUpdates(CurrencyUpdate[] updates, List<Contracts.Currency> currencies)
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