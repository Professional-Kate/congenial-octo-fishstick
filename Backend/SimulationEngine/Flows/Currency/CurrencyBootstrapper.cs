using IdelPog.Common.Enums;
using IdelPog.Common.Repository;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Messenger;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.SimulationEngine.Currency.Factories;
using IdelPog.SimulationEngine.Currency.Listeners;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyBootstrapper
    {
        public void Initialize(IBufferMessenger bufferMessenger, IBufferManager bufferManager)
        {
            IAssertPositive assertPositive = new AssertPositive(new ThrowHandler());
            IAssertCollectionNotEmpty assertCollectionNotEmpty = new AssertCollectionNotEmpty(new ThrowHandler());
            IAssertNotNull assertNotNull = new AssertNotNull(new ThrowHandler());
            IAssertNonDuplicate assertNonDuplicate = new AssertNonDuplicate(new ThrowHandler());
            IAssertFound assertFound = new AssertFound(new ThrowHandler());
            IAssertEnoughCurrency assertEnoughCurrency = new AssertEnoughCurrency(new ThrowHandler());
            
            IStateRepository<CurrencyType, Currency> currencyRepository = new StateRepository<CurrencyType, Currency>();

            ICurrencyService currencyService = new CurrencyService(assertPositive, assertEnoughCurrency);
            ICurrencyUpdateDTOFactory currencyUpdateDTOFactory = new CurrencyUpdateDTOFactory(assertNotNull, assertCollectionNotEmpty);
            IDispatchMany<CurrencyUpdateDTO> currencyUpdateDispatcher = new ManagedDispatcher<CurrencyUpdateDTO>(bufferManager, assertNotNull, assertCollectionNotEmpty);
            ICurrencyUpdateFactory currencyUpdateFactory = new CurrencyUpdateFactory();
            ICurrencyUpdateSummarizer currencyUpdateSummarizer = new CurrencyUpdateSummarizer(currencyUpdateFactory, assertPositive, assertNotNull, assertCollectionNotEmpty);
            ICurrencyUpdateMediator currencyUpdateMediator = new CurrencyUpdateMediator(currencyRepository, currencyService, currencyUpdateDispatcher, currencyUpdateSummarizer, currencyUpdateDTOFactory, assertPositive, assertCollectionNotEmpty, assertFound, assertNotNull);
            
            ICurrencyCreationDTOFactory currencyCreationDTOFactory = new CurrencyCreationDTOFactory(assertNotNull, assertCollectionNotEmpty);
            IDispatchMany<CurrencyCreationDTO> currencyCreationDispatcher = new ManagedDispatcher<CurrencyCreationDTO>(bufferManager, assertNotNull, assertCollectionNotEmpty);
            ICurrencyCreationMediator currencyCreationMediator = new CurrencyCreationMediator(currencyRepository, currencyCreationDispatcher, currencyCreationDTOFactory,  assertNotNull, assertCollectionNotEmpty,  assertNonDuplicate, assertPositive);
            
            ICurrencyController currencyController = new CurrencyController(currencyUpdateMediator, currencyCreationMediator);
            IErrorFactory errorFactory = new ErrorFactory();

            ICurrencyUpdateErrorDTOFactory currencyUpdateErrorDTOFactory = new CurrencyUpdateErrorDTOFactory(errorFactory, currencyUpdateDTOFactory);
            IDispatchOne<CurrencyUpdateErrorDTO> currencyUpdateErrorDispatcher = new ManagedDispatcher<CurrencyUpdateErrorDTO>(bufferManager, assertNotNull, assertCollectionNotEmpty);
            CurrencyUpdateListener currencyUpdateListener = new(currencyController, currencyUpdateErrorDispatcher, currencyUpdateErrorDTOFactory);

            ICurrencyCreationErrorDTOFactory currencyCreationErrorDTOFactory = new CurrencyCreationErrorDTOFactory(errorFactory, currencyCreationDTOFactory);
            IDispatchOne<CurrencyCreationErrorDTO> currencyCreationErrorDispatcher = new ManagedDispatcher<CurrencyCreationErrorDTO>(bufferManager, assertNotNull, assertCollectionNotEmpty);
            CurrencyCreationListener currencyCreationListener = new(currencyController, currencyCreationErrorDispatcher, currencyCreationErrorDTOFactory);
            
            bufferMessenger.Subscribe(currencyUpdateListener);
            bufferMessenger.Subscribe(currencyCreationListener);
        }
    }
}