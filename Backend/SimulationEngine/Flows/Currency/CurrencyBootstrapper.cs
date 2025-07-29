using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Common.Repository;
using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Messaging.Messenger;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.SimulationEngine.Currency.Factories;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyBootstrapper
    {
        public void Initialize(IBufferMessenger bufferMessenger, IBufferManager bufferManager)
        {
            IHandler throwHandler = new ThrowHandler();
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            ICurrencyAssertion currencyAssertion = new CurrencyAssertion(throwHandler);

            IStateRepository<CurrencyType, Models.Currency> currencyRepository = new StateRepository<CurrencyType, Models.Currency>();

            ICurrencyService currencyService = new CurrencyService(currencyAssertion);
            ICurrencyUpdateDTOFactory currencyUpdateDTOFactory = new CurrencyUpdateDTOFactory(objectNullAssertion, collectionAssertion);
            IDispatchMany<CurrencyUpdateDTO> currencyUpdateDispatcher = new ManagedDispatcher<CurrencyUpdateDTO>(bufferManager, objectNullAssertion, collectionAssertion);

            ICurrencyUpdateFactory currencyUpdateFactory = new CurrencyUpdateFactory();
            ICurrencyUpdateSummarizer currencyUpdateSummarizer = new CurrencyUpdateSummarizer(currencyUpdateFactory, objectNullAssertion, collectionAssertion);

            ICurrencyUpdateMediator currencyUpdateMediator = new CurrencyUpdateMediator(currencyRepository, currencyService, currencyUpdateDispatcher, currencyUpdateSummarizer, currencyUpdateDTOFactory, collectionAssertion, foundAssertion, objectNullAssertion);

            ICurrencyCreationDTOFactory currencyCreationDTOFactory = new CurrencyCreationDTOFactory(objectNullAssertion, collectionAssertion);
            IDispatchMany<CurrencyCreationDTO> currencyCreationDispatcher = new ManagedDispatcher<CurrencyCreationDTO>(bufferManager, objectNullAssertion, collectionAssertion);

            ICurrencyCreationMediator currencyCreationMediator = new CurrencyCreationMediator(currencyRepository, currencyCreationDispatcher, currencyCreationDTOFactory, objectNullAssertion, collectionAssertion, uniqueAssertion);
            
            IErrorDTOFactory errorDTOFactory = new ErrorDTOFactory();
            IErrorFactory<CurrencyUpdateErrorDTO, IReadOnlyList<CurrencyUpdate>> currencyUpdateErrorDTOFactory = new CurrencyUpdateErrorDTOFactory(errorDTOFactory, currencyUpdateDTOFactory);
            IDispatchOne<CurrencyUpdateErrorDTO> currencyUpdateErrorDispatcher = new ManagedDispatcher<CurrencyUpdateErrorDTO>(bufferManager, objectNullAssertion, collectionAssertion);

            IErrorFactory<CurrencyCreationErrorDTO, IReadOnlyList<CurrencyCreation>> currencyCreationErrorDTOFactory = new CurrencyCreationErrorDTOFactory(errorDTOFactory, currencyCreationDTOFactory);
            IDispatchOne<CurrencyCreationErrorDTO> currencyCreationErrorDispatcher = new ManagedDispatcher<CurrencyCreationErrorDTO>(bufferManager, objectNullAssertion, collectionAssertion);
            
            IContextualHandler<IReadOnlyList<CurrencyUpdate>> updateDispatchHandler = new DispatchingHandler<CurrencyUpdateErrorDTO, IReadOnlyList<CurrencyUpdate>>(currencyUpdateErrorDispatcher, currencyUpdateErrorDTOFactory);
            IBatchControllerExecutionAssertion<CurrencyUpdate> updateExecutionAssertion = new BatchControllerExecutionAssertion<CurrencyUpdate>(updateDispatchHandler);
            
            IContextualHandler<IReadOnlyList<CurrencyCreation>> createDispatchHandler = new DispatchingHandler<CurrencyCreationErrorDTO, IReadOnlyList<CurrencyCreation>>(currencyCreationErrorDispatcher, currencyCreationErrorDTOFactory);
            IBatchControllerExecutionAssertion<CurrencyCreation> createExecutionAssertion = new BatchControllerExecutionAssertion<CurrencyCreation>(createDispatchHandler);
            
            IBatchController<CurrencyUpdate> currencyController = new CurrencyUpdateController(currencyUpdateMediator);
            IBufferListener<CurrencyUpdate> currencyUpdateListener = new ManagedBufferListener<CurrencyUpdate>(currencyController, updateExecutionAssertion);
            
            IBatchController<CurrencyCreation> currencyCreationController = new CurrencyCreationController(currencyCreationMediator);
            IBufferListener<CurrencyCreation> currencyCreationListener = new ManagedBufferListener<CurrencyCreation>(currencyCreationController, createExecutionAssertion);
            
            bufferMessenger.Subscribe(currencyUpdateListener);
            bufferMessenger.Subscribe(currencyCreationListener);
        }
    }
}