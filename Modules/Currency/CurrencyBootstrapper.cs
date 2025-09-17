using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Factory;
using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Flows.Registry;
using IdelPog.Core.Logging;
using IdelPog.Core.Logging.Writer;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Controller;
using IdelPog.Core.Messaging.Dispatcher;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Currency.Assertion;
using IdelPog.Currency.Assertion.Interface;
using IdelPog.Currency.Contracts;
using IdelPog.Currency.Factory;
using IdelPog.Currency.Factory.Interface;
using IdelPog.Currency.Mediator;
using IdelPog.Currency.Service;
using IdelPog.Currency.Service.Interface;

namespace IdelPog.Currency
{
    public static class CurrencyBootstrapper
    {
        /// <summary>
        /// Creates and adds the <see cref="CurrencyCreation"/> and <see cref="CurrencyUpdate"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="flowRegistry">Used to register the CurrencyCreation and CurrencyUpdate flows</param>
        /// <seealso cref="RegisterCurrencyCreation"/>
        /// <seealso cref="RegisterCurrencyUpdate"/>
        public static void RegisterFlows(IBufferManager bufferManager, IBatchRegister flowRegistry)
        {
            IStateRepository<CurrencyType, Contracts.Currency> currencyRepository = new StateRepository<CurrencyType, Contracts.Currency>();

            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            
            ILogWriter writer = new ConsoleWriter();
            IBufferLogger bufferLogger = new BufferLoggingService(writer);
            
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();

            RegisterCurrencyCreation(bufferManager, currencyRepository, baseErrorFactory, throwHandler, objectNullAssertion, collectionAssertion, flowRegistry, bufferLogger);
            RegisterCurrencyUpdate(bufferManager,  currencyRepository, baseErrorFactory, throwHandler, objectNullAssertion, collectionAssertion, flowRegistry, bufferLogger);
        }

        /// <summary>
        /// Registers the <see cref="CurrencyCreation"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="CurrencyCreationResponse"/></param>
        /// <param name="currencyRepository">Used to store all <see cref="Currency"/> models</param>
        /// <param name="throwHandler">The handle used in all assertions</param>
        /// <param name="baseErrorFactory">Used to construct <see cref="BaseError"/></param>
        /// <param name="objectNullAssertion">Used to assert if objects are null</param>
        /// <param name="collectionAssertion">Used to assert if a collection is null or empty</param>
        /// <param name="flowRegistry">Used to register the CurrencyCreation flow</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// /// <remarks>
        /// Listens to -> <see cref="CurrencyCreation"/>. On Success -> <see cref="CurrencyCreationResponse"/>. On Error -> <see cref="CurrencyCreationError"/>
        /// </remarks>
        private static void RegisterCurrencyCreation(IBufferManager bufferManager,
            IStateRepository<CurrencyType, Contracts.Currency> currencyRepository, IBaseErrorFactory baseErrorFactory, IHandler throwHandler, IObjectNullAssertion objectNullAssertion,
            ICollectionAssertion collectionAssertion, IBatchRegister flowRegistry, IBufferLogger bufferLogger)
        {
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);

            ICurrencyCreationResponseFactory currencyCreationResponseFactory = new CurrencyCreationResponseFactory(objectNullAssertion, collectionAssertion);

            IDispatchOne<CurrencyCreationResponse> currencyCreationResponseDispatcher = new ManagedDispatcher<CurrencyCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            IBatchMediator<CurrencyCreation> currencyCreationMediator = new CurrencyCreationMediator(currencyRepository, currencyCreationResponseDispatcher, currencyCreationResponseFactory, objectNullAssertion,  collectionAssertion, uniqueAssertion);
            IBatchController<CurrencyCreation> currencyCreationController = new ManagedBatchController<CurrencyCreation>(currencyCreationMediator);
            
            IErrorFactory<CurrencyCreationError, IReadOnlyList<CurrencyCreation>> currencyCreationErrorFactory = new CurrencyCreationErrorFactory(baseErrorFactory);
            
            flowRegistry.RegisterBatch(currencyCreationController, currencyCreationErrorFactory);
        }

        /// <summary>
        /// Registers the <see cref="CurrencyUpdate"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="CurrencyUpdateError"/> if anything is thrown</param>
        /// <param name="currencyRepository">Used to store all <see cref="Currency"/> models</param>
        /// <param name="throwHandler">The handle used in all assertions</param>
        /// <param name="baseErrorFactory">Used to construct <see cref="BaseError"/></param>
        /// <param name="objectNullAssertion">Used to assert if objects are null</param>
        /// <param name="collectionAssertion">Used to assert if a collection is null or empty</param>
        /// <param name="flowRegistry">Used to register the CurrencyUpdate flow</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// /// <remarks>
        /// Listens to -> <see cref="CurrencyUpdate"/>. On Success -> <see cref="CurrencyUpdateResponse"/>. On Error -> <see cref="CurrencyUpdateError"/>
        /// </remarks>
        private static void RegisterCurrencyUpdate(IBufferManager bufferManager,
            IStateRepository<CurrencyType, Contracts.Currency> currencyRepository, IBaseErrorFactory baseErrorFactory, IHandler throwHandler, IObjectNullAssertion objectNullAssertion,
            ICollectionAssertion collectionAssertion, IBatchRegister flowRegistry, IBufferLogger bufferLogger)
        {
            ICurrencyAssertion currencyAssertion = new CurrencyAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            
            ICurrencyUpdateFactory updateFactory = new CurrencyUpdateFactory();
            
            ICurrencyService currencyService = new CurrencyService(currencyAssertion);
            IDispatchOne<CurrencyUpdateResponse> updateResponseDispatcher = new ManagedDispatcher<CurrencyUpdateResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            ICurrencyUpdateSummarizer currencyUpdateSummarizer = new CurrencyUpdateSummarizer(updateFactory, collectionAssertion);
            ICurrencyUpdateResponseFactory updateResponseFactory = new CurrencyUpdateResponseFactory(objectNullAssertion, collectionAssertion);

            IBatchMediator<CurrencyUpdate> updateMediator = new CurrencyUpdateMediator(currencyRepository, currencyService, updateResponseDispatcher, currencyUpdateSummarizer, updateResponseFactory, collectionAssertion, foundAssertion, objectNullAssertion);
            IBatchController<CurrencyUpdate> updateController = new ManagedBatchController<CurrencyUpdate>(updateMediator);
            
            IErrorFactory<CurrencyUpdateError, IReadOnlyList<CurrencyUpdate>> updateErrorFactory = new CurrencyUpdateErrorFactory(baseErrorFactory);
            
            flowRegistry.RegisterBatch(updateController, updateErrorFactory);
        }
    }
}