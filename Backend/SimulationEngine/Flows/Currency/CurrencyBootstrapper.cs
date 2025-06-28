using IdelPog.Common.Repository;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Flows.Currency.Assertions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.SimulationEngine.Flows.Currency
{
    public class CurrencyBootstrapper
    {
        public void Initialize(IBufferMessenger bufferMessenger, IBufferManager bufferManager)
        {
            IAssertPositive assertPositive = new AssertPositive(new ThrowHandler());
            AssertCollectionNotEmpty assertCollectionNotEmpty = new(new ThrowHandler());
            AssertNotNull assertNotNull = new(new ThrowHandler());
            
            ICurrencyService currencyService = new CurrencyService();
            IStateRepository<CurrencyType, Currency> currencyRepository = new StateRepository<CurrencyType, Currency>();
            ICurrencyUpdateDispatcher currencyUpdateDispatcher = new CurrencyUpdateDispatcher(bufferManager);
            ICurrencyUpdateFactory currencyUpdateFactory = new CurrencyUpdateFactory(assertNotNull, assertCollectionNotEmpty);
            
            ICurrencyMediator currencyMediator = new CurrencyMediator(currencyService,  currencyRepository, currencyUpdateDispatcher, assertPositive, assertCollectionNotEmpty, currencyUpdateFactory);
            ICurrencyController currencyController = new CurrencyController(currencyMediator);
            CurrencyTradeListener currencyTradeListener = new(currencyController);
            CreateBasicCurrency(currencyRepository);
            
            bufferMessenger.Subscribe(currencyTradeListener);
        }

        private static void CreateBasicCurrency(IStateRepository<CurrencyType, Currency> repository)
        {
            repository.Add(CurrencyType.GOLD, new Currency(CurrencyType.GOLD, 0));
            repository.Add(CurrencyType.GEMS, new Currency(CurrencyType.GEMS, 0));
        }
    }
}