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
            IAssertCollectionNotEmpty assertCollectionNotEmpty = new AssertCollectionNotEmpty(new ThrowHandler());
            IAssertNotNull assertNotNull = new AssertNotNull(new ThrowHandler());
            
            IStateRepository<CurrencyType, Currency> currencyRepository = new StateRepository<CurrencyType, Currency>();
            
            ICurrencyService currencyService = new CurrencyService();
            ICurrencyUpdateFactory currencyUpdateFactory = new CurrencyUpdateFactory(assertNotNull, assertCollectionNotEmpty);
            ICurrencyUpdateDispatcher currencyUpdateDispatcher = new CurrencyUpdateDispatcher(bufferManager, currencyUpdateFactory);
            ICurrencyUpdateMediator currencyUpdateMediator = new CurrencyUpdateMediator(currencyService,  currencyRepository, currencyUpdateDispatcher, assertPositive, assertCollectionNotEmpty);
            
            ICurrencyCreationFactory currencyCreationFactory = new CurrencyCreationFactory(assertNotNull, assertCollectionNotEmpty);
            ICurrencyCreationDispatcher currencyCreationDispatcher = new CurrencyCreationDispatcher(bufferManager, currencyCreationFactory);
            ICurrencyCreationMediator currencyCreationMediator = new CurrencyCreationMediator(currencyRepository, currencyCreationDispatcher,  assertNotNull, assertCollectionNotEmpty);
            
            ICurrencyController currencyController = new CurrencyController(currencyUpdateMediator, currencyCreationMediator);
            CurrencyTradeListener currencyTradeListener = new(currencyController);
            CurrencyCreationListener currencyCreationListener = new(currencyController);
            
            bufferMessenger.Subscribe(currencyTradeListener);
            bufferMessenger.Subscribe(currencyCreationListener);
        }
    }
}