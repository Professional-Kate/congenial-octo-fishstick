using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
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
using CurrencyUpdateFactory = IdelPog.SimulationEngine.Currency.Factories.CurrencyUpdateFactory;
using ICurrencyUpdateFactory = IdelPog.SimulationEngine.Currency.Factories.ICurrencyUpdateFactory;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyBootstrapper
    {
        public void Initialize(IBufferMessenger bufferMessenger, IBufferManager bufferManager)
        {
            ICollectionAssertion collectionAssertion = new CollectionAssertion(new ThrowHandler());
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(new ThrowHandler());
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(new ThrowHandler());
            IFoundAssertion foundAssertion = new FoundAssertion(new ThrowHandler());
            ICurrencyAssertion currencyAssertion = new CurrencyAssertion(new ThrowHandler());

            IStateRepository<CurrencyType, Models.Currency> currencyRepository = new StateRepository<CurrencyType, Models.Currency>();

            ICurrencyService currencyService = new CurrencyService(currencyAssertion);
            ICurrencyUpdateDTOFactory currencyUpdateDTOFactory = new CurrencyUpdateDTOFactory(objectNullAssertion, collectionAssertion);
            IDispatchMany<CurrencyUpdateDTO> currencyUpdateDispatcher =
                new ManagedDispatcher<CurrencyUpdateDTO>(bufferManager, objectNullAssertion, collectionAssertion);

            ICurrencyUpdateFactory currencyUpdateFactory = new CurrencyUpdateFactory();
            ICurrencyUpdateSummarizer currencyUpdateSummarizer =
                new CurrencyUpdateSummarizer(currencyUpdateFactory, objectNullAssertion, collectionAssertion);

            ICurrencyUpdateMediator currencyUpdateMediator = new CurrencyUpdateMediator(currencyRepository, currencyService, currencyUpdateDispatcher,
                currencyUpdateSummarizer, currencyUpdateDTOFactory, collectionAssertion, foundAssertion, objectNullAssertion);

            ICurrencyCreationDTOFactory currencyCreationDTOFactory = new CurrencyCreationDTOFactory(objectNullAssertion, collectionAssertion);
            IDispatchMany<CurrencyCreationDTO> currencyCreationDispatcher =
                new ManagedDispatcher<CurrencyCreationDTO>(bufferManager, objectNullAssertion, collectionAssertion);

            ICurrencyCreationMediator currencyCreationMediator = new CurrencyCreationMediator(currencyRepository, currencyCreationDispatcher,
                currencyCreationDTOFactory, objectNullAssertion, collectionAssertion, uniqueAssertion);

            ICurrencyController currencyController = new CurrencyController(currencyUpdateMediator, currencyCreationMediator);
            IErrorDTOFactory errorDTOFactory = new ErrorDTOFactory();

            ICurrencyUpdateErrorDTOFactory currencyUpdateErrorDTOFactory = new CurrencyUpdateErrorDTOFactory(errorDTOFactory, currencyUpdateDTOFactory);
            IDispatchOne<CurrencyUpdateErrorDTO> currencyUpdateErrorDispatcher =
                new ManagedDispatcher<CurrencyUpdateErrorDTO>(bufferManager, objectNullAssertion, collectionAssertion);

            CurrencyUpdateListener currencyUpdateListener = new(currencyController, currencyUpdateErrorDispatcher, currencyUpdateErrorDTOFactory);

            ICurrencyCreationErrorDTOFactory currencyCreationErrorDTOFactory = new CurrencyCreationErrorDTOFactory(errorDTOFactory, currencyCreationDTOFactory);
            IDispatchOne<CurrencyCreationErrorDTO> currencyCreationErrorDispatcher =
                new ManagedDispatcher<CurrencyCreationErrorDTO>(bufferManager, objectNullAssertion, collectionAssertion);

            CurrencyCreationListener currencyCreationListener = new(currencyController, currencyCreationErrorDispatcher, currencyCreationErrorDTOFactory);

            bufferMessenger.Subscribe(currencyUpdateListener);
            bufferMessenger.Subscribe(currencyCreationListener);
        }
    }
}