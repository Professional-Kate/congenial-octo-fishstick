using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Common.Repository;
using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Dispatch.Buffer;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Messaging.Messenger;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Factories;
using IdelPog.SimulationEngine.Currency.Responses;
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
            ICurrencyUpdateResponseFactory currencyUpdateResponseFactory = new CurrencyUpdateResponseFactory(objectNullAssertion, collectionAssertion);
            IDispatchMany<CurrencyUpdateResponse> currencyUpdateDispatcher = new ManagedDispatcher<CurrencyUpdateResponse>(bufferManager, objectNullAssertion, collectionAssertion);

            ICurrencyUpdateFactory currencyUpdateFactory = new CurrencyUpdateFactory();
            ICurrencyUpdateSummarizer currencyUpdateSummarizer = new CurrencyUpdateSummarizer(currencyUpdateFactory, objectNullAssertion, collectionAssertion);

            IBatchMediator<CurrencyUpdate> currencyUpdateMediator = new CurrencyUpdateMediator(currencyRepository, currencyService, currencyUpdateDispatcher, currencyUpdateSummarizer, currencyUpdateResponseFactory, collectionAssertion, foundAssertion, objectNullAssertion);

            ICurrencyCreationResponseFactory currencyCreationResponseFactory = new CurrencyCreationResponseFactory(objectNullAssertion, collectionAssertion);
            IDispatchMany<CurrencyCreationResponse> currencyCreationDispatcher = new ManagedDispatcher<CurrencyCreationResponse>(bufferManager, objectNullAssertion, collectionAssertion);

            IBatchMediator<CurrencyCreation> currencyCreationMediator = new CurrencyCreationMediator(currencyRepository, currencyCreationDispatcher, currencyCreationResponseFactory, objectNullAssertion, collectionAssertion, uniqueAssertion);
            
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<CurrencyUpdateError, IReadOnlyList<CurrencyUpdate>> currencyUpdateErrorDTOFactory = new CurrencyUpdateErrorFactory(baseErrorFactory, currencyUpdateResponseFactory);
            IDispatchOne<CurrencyUpdateError> currencyUpdateErrorDispatcher = new ManagedDispatcher<CurrencyUpdateError>(bufferManager, objectNullAssertion, collectionAssertion);

            IErrorFactory<CurrencyCreationError, IReadOnlyList<CurrencyCreation>> currencyCreationErrorDTOFactory = new CurrencyCreationErrorFactory(baseErrorFactory, currencyCreationResponseFactory);
            IDispatchOne<CurrencyCreationError> currencyCreationErrorDispatcher = new ManagedDispatcher<CurrencyCreationError>(bufferManager, objectNullAssertion, collectionAssertion);
            
            IContextualHandler<IReadOnlyList<CurrencyUpdate>> updateDispatchHandler = new DispatchingHandler<CurrencyUpdateError, IReadOnlyList<CurrencyUpdate>>(currencyUpdateErrorDispatcher, currencyUpdateErrorDTOFactory);
            IBatchControllerExecutionAssertion<CurrencyUpdate> updateExecutionAssertion = new BatchControllerExecutionAssertion<CurrencyUpdate>(updateDispatchHandler);
            
            IContextualHandler<IReadOnlyList<CurrencyCreation>> createDispatchHandler = new DispatchingHandler<CurrencyCreationError, IReadOnlyList<CurrencyCreation>>(currencyCreationErrorDispatcher, currencyCreationErrorDTOFactory);
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