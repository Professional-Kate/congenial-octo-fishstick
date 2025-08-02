using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Errors;
using IdelPog.Common.Factories;
using IdelPog.Common.Repository;
using IdelPog.Common.Responses;
using IdelPog.Flows.Builder;
using IdelPog.Flows.Types;
using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Controller;
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
using IdelPog.SimulationEngine.Skill;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.SimulationEngine.Currency
{
    public static class CurrencyBootstrapper
    {
        /// <summary>
        /// Creates and adds the <see cref="CurrencyCreation"/> and <see cref="CurrencyUpdate"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="flowDescriptorDispatcher">Used to dispatch a <see cref="FlowDescriptor"/></param>
        /// <seealso cref="RegisterCurrencyCreation"/>
        /// <seealso cref="RegisterCurrencyUpdate"/>
        public static void RegisterFlows(IBufferManager bufferManager, IDispatchOne<FlowDescriptor> flowDescriptorDispatcher)
        {
            IStateRepository<CurrencyType, Models.Currency> currencyRepository = new StateRepository<CurrencyType, Models.Currency>();

            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();

            RegisterCurrencyCreation(bufferManager, flowDescriptorDispatcher, currencyRepository, baseErrorFactory, throwHandler, objectNullAssertion, collectionAssertion);
            RegisterCurrencyUpdate(bufferManager, flowDescriptorDispatcher,  currencyRepository, baseErrorFactory, throwHandler, objectNullAssertion, collectionAssertion);
        }

        /// <summary>
        /// Registers the <see cref="CurrencyCreation"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="CurrencyCreationResponse"/></param>
        /// <param name="flowDescriptorDispatcher">Used to dispatch a <see cref="FlowDescriptor"/></param>
        /// <param name="currencyRepository">Used to store all <see cref="Currency"/> models</param>
        /// <param name="throwHandler">The handle used in all assertions</param>
        /// <param name="baseErrorFactory">Used to construct <see cref="BaseError"/></param>
        /// <param name="objectNullAssertion">Used to assert if objects are null</param>
        /// <param name="collectionAssertion">Used to assert if a collection is null or empty</param>
        /// /// <remarks>
        /// Listens to -> <see cref="CurrencyCreation"/>. On Success -> <see cref="CurrencyCreationResponse"/>. On Error -> <see cref="CurrencyCreationError"/>
        /// </remarks>
        private static void RegisterCurrencyCreation(IBufferManager bufferManager, IDispatchOne<FlowDescriptor> flowDescriptorDispatcher,
            IStateRepository<CurrencyType, Models.Currency> currencyRepository, IBaseErrorFactory baseErrorFactory, IHandler throwHandler, IObjectNullAssertion objectNullAssertion,
            ICollectionAssertion collectionAssertion)
        {
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);
            
            IDispatchOne<CurrencyCreationError> currencyCreationErrorDispatcher = new ManagedDispatcher<CurrencyCreationError>(bufferManager, objectNullAssertion, collectionAssertion);
            ICurrencyCreationResponseFactory currencyCreationResponseFactory = new CurrencyCreationResponseFactory(objectNullAssertion, collectionAssertion);

            IDispatchOne<CurrencyCreationResponse> currencyCreationResponseDispatcher = new ManagedDispatcher<CurrencyCreationResponse>(bufferManager, objectNullAssertion, collectionAssertion);
            IBatchMediator<CurrencyCreation> currencyCreationMediator = new CurrencyCreationMediator(currencyRepository, currencyCreationResponseDispatcher, currencyCreationResponseFactory, objectNullAssertion,  collectionAssertion, uniqueAssertion);
            IBatchController<CurrencyCreation> currencyCreationController = new ManagedBatchController<CurrencyCreation>(currencyCreationMediator);
            
            IErrorFactory<CurrencyCreationError, IReadOnlyList<CurrencyCreation>> currencyCreationErrorFactory = new CurrencyCreationErrorFactory(baseErrorFactory);
            
            FlowDescriptor flowDescriptor = new FlowBuilder()
                .ForCommand(typeof(CurrencyCreation))
                .SetDispatchMode(BufferMode.BATCH)
                .SetDescription(typeof(CurrencyCreation), typeof(CurrencyCreationResponse), typeof(CurrencyCreationError))
                .WithController(currencyCreationController)
                .WithResponseDispatcher(currencyCreationErrorDispatcher)
                .WithErrorFactory(currencyCreationErrorFactory)
                .Build();
            
            flowDescriptorDispatcher.Dispatch(flowDescriptor);
        }

        /// <summary>
        /// Registers the <see cref="CurrencyUpdate"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="CurrencyUpdateError"/> if anything is thrown</param>
        /// <param name="flowDescriptorDispatcher">Used to dispatch a <see cref="FlowDescriptor"/></param>
        /// <param name="currencyRepository">Used to store all <see cref="Currency"/> models</param>
        /// <param name="throwHandler">The handle used in all assertions</param>
        /// <param name="baseErrorFactory">Used to construct <see cref="BaseError"/></param>
        /// <param name="objectNullAssertion">Used to assert if objects are null</param>
        /// <param name="collectionAssertion">Used to assert if a collection is null or empty</param>
        /// /// <remarks>
        /// Listens to -> <see cref="CurrencyUpdate"/>. On Success -> <see cref="CurrencyUpdateResponse"/>. On Error -> <see cref="CurrencyUpdateError"/>
        /// </remarks>
        private static void RegisterCurrencyUpdate(IBufferManager bufferManager, IDispatchOne<FlowDescriptor> flowDescriptorDispatcher,
            IStateRepository<CurrencyType, Models.Currency> currencyRepository, IBaseErrorFactory baseErrorFactory, IHandler throwHandler, IObjectNullAssertion objectNullAssertion,
            ICollectionAssertion collectionAssertion)
        {
            ICurrencyAssertion currencyAssertion = new CurrencyAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            
            ICurrencyUpdateFactory updateFactory = new CurrencyUpdateFactory();
            
            ICurrencyService currencyService = new CurrencyService(currencyAssertion);
            IDispatchOne<CurrencyUpdateResponse> updateResponseDispatcher = new ManagedDispatcher<CurrencyUpdateResponse>(bufferManager, objectNullAssertion, collectionAssertion);
            ICurrencyUpdateSummarizer currencyUpdateSummarizer = new CurrencyUpdateSummarizer(updateFactory, objectNullAssertion, collectionAssertion);
            ICurrencyUpdateResponseFactory updateResponseFactory = new CurrencyUpdateResponseFactory(objectNullAssertion, collectionAssertion);
            IDispatchOne<CurrencyUpdateError> updateErrorDispatcher = new ManagedDispatcher<CurrencyUpdateError>(bufferManager, objectNullAssertion, collectionAssertion);
            
            IBatchMediator<CurrencyUpdate> updateMediator = new CurrencyUpdateMediator(currencyRepository, currencyService, updateResponseDispatcher, currencyUpdateSummarizer, updateResponseFactory, collectionAssertion, foundAssertion, objectNullAssertion);
            IBatchController<CurrencyUpdate> updateController = new ManagedBatchController<CurrencyUpdate>(updateMediator);
            
            IErrorFactory<CurrencyUpdateError, IReadOnlyList<CurrencyUpdate>> currencyCreationErrorFactory = new CurrencyUpdateErrorFactory(baseErrorFactory);
            
            FlowDescriptor flowDescriptor = new FlowBuilder()
                .ForCommand(typeof(CurrencyUpdate))
                .SetDispatchMode(BufferMode.BATCH)
                .SetDescription(typeof(CurrencyUpdate), typeof(CurrencyUpdateResponse), typeof(CurrencyUpdateError))
                .WithController(updateController)
                .WithResponseDispatcher(updateErrorDispatcher)
                .WithErrorFactory(currencyCreationErrorFactory)
                .Build();
            
            flowDescriptorDispatcher.Dispatch(flowDescriptor);
        }
    }
}