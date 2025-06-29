using IdelPog.Common.Repository;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Listeners;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.SimulationEngine.Currency
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
            ICurrencyUpdateFactory currencyUpdateFactory = new CurrencyUpdateFactory(assertNotNull, assertCollectionNotEmpty);
            ICurrencyUpdateDispatcher currencyUpdateDispatcher = new CurrencyUpdateDispatcher(bufferManager, currencyUpdateFactory);
            
            ICurrencyMediator currencyMediator = new CurrencyMediator(currencyService,  currencyRepository, currencyUpdateDispatcher, assertPositive, assertCollectionNotEmpty);
            ICurrencyController currencyController = new CurrencyController(currencyMediator);
            CurrencyTradeListener currencyTradeListener = new(currencyController);
            
            bufferMessenger.Subscribe(currencyTradeListener);
        }
    }
}