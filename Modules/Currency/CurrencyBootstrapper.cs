using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Factory;
using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Flows.Registry;
using IdelPog.Core.Logging;
using IdelPog.Core.Logging.Writer;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Controller;
using IdelPog.Core.Messaging.Dispatcher;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Currency.Assertion;
using IdelPog.Currency.Assertion.Interface;
using IdelPog.Currency.Contracts;
using IdelPog.Currency.Contracts.Command;
using IdelPog.Currency.Contracts.Error;
using IdelPog.Currency.Contracts.Response;
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
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IFoundAssertion foundAssertion = new FoundAssertion();
            ICurrencyAssertion currencyAssertion = new CurrencyAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(foundAssertion, objectNullAssertion, uniqueAssertion);
            
            IStateRepository<CurrencyType, Contracts.Currency> currencyRepository = new StateRepository<CurrencyType, Contracts.Currency>(repositoryAsserter);
            
            ICurrencyService currencyService = new CurrencyService(currencyAssertion);
            ICurrencyUpdateFactory updateFactory = new CurrencyUpdateFactory();
            ICurrencyUpdateResponseFactory updateResponseFactory = new CurrencyUpdateResponseFactory(objectNullAssertion, collectionAssertion);
            ICurrencyUpdateSummarizer currencyUpdateSummarizer = new CurrencyUpdateSummarizer(updateFactory, collectionAssertion);
            ICurrencyUpdateService currencyUpdateService = new CurrencyUpdateService(currencyService, currencyRepository, collectionAssertion, foundAssertion, updateResponseFactory, currencyUpdateSummarizer);
            
            ILogWriter writer = new ConsoleWriter();
            IBufferLogger bufferLogger = new BufferLoggingService(writer);
            
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();

            RegisterCurrencyCreation(bufferManager, currencyRepository, baseErrorFactory, objectNullAssertion, collectionAssertion, flowRegistry, bufferLogger);
            RegisterCurrencyUpdate(bufferManager, baseErrorFactory, objectNullAssertion, collectionAssertion, flowRegistry, bufferLogger, currencyUpdateService);
        }

        /// <summary>
        /// Registers the <see cref="CurrencyCreation"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="CurrencyCreationResponse"/></param>
        /// <param name="currencyRepository">Used to store all <see cref="Currency"/> models</param>
        /// <param name="baseErrorFactory">Used to construct <see cref="BaseError"/></param>
        /// <param name="objectNullAssertion">Used to assert if objects are null</param>
        /// <param name="collectionAssertion">Used to assert if a collection is null or empty</param>
        /// <param name="flowRegistry">Used to register the CurrencyCreation flow</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// /// <remarks>
        /// Listens to -> <see cref="CurrencyCreation"/>. On Success -> <see cref="CurrencyCreationResponse"/>. On Error -> <see cref="CurrencyCreationError"/>
        /// </remarks>
        private static void RegisterCurrencyCreation(IBufferManager bufferManager,
            IStateRepository<CurrencyType, Contracts.Currency> currencyRepository, IBaseErrorFactory baseErrorFactory, IObjectNullAssertion objectNullAssertion,
            ICollectionAssertion collectionAssertion, IBatchRegister flowRegistry, IBufferLogger bufferLogger)
        {
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();

            ICurrencyCreationResponseFactory currencyCreationResponseFactory = new CurrencyCreationResponseFactory(objectNullAssertion, collectionAssertion);

            IDispatchMany<CurrencyCreationResponse> currencyCreationResponseDispatcher = new ManagedDispatcher<CurrencyCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            IBatchMediator<CurrencyCreation> currencyCreationMediator = new CurrencyCreationMediator(currencyRepository, currencyCreationResponseDispatcher, currencyCreationResponseFactory, objectNullAssertion,  collectionAssertion, uniqueAssertion);
            IBatchController<CurrencyCreation> currencyCreationController = new ManagedBatchController<CurrencyCreation>(currencyCreationMediator);
            
            IErrorFactory<CurrencyCreationError, IReadOnlyList<CurrencyCreation>> currencyCreationErrorFactory = new CurrencyCreationErrorFactory(baseErrorFactory);
            
            flowRegistry.RegisterBatch(currencyCreationController, currencyCreationErrorFactory);
        }

        /// <summary>
        /// Registers the <see cref="CurrencyUpdate"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="CurrencyUpdateError"/> if anything is thrown</param>
        /// <param name="baseErrorFactory">Used to construct <see cref="BaseError"/></param>
        /// <param name="objectNullAssertion">Used to assert if objects are null</param>
        /// <param name="collectionAssertion">Used to assert if a collection is null or empty</param>
        /// <param name="flowRegistry">Used to register the CurrencyUpdate flow</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <param name="currencyUpdateService">Used to update <see cref="Currency"/></param>
        /// /// <remarks>
        /// Listens to -> <see cref="CurrencyUpdate"/>. On Success -> <see cref="CurrencyUpdateResponse"/>. On Error -> <see cref="CurrencyUpdateError"/>
        /// </remarks>
        private static void RegisterCurrencyUpdate(IBufferManager bufferManager, IBaseErrorFactory baseErrorFactory, IObjectNullAssertion objectNullAssertion, ICollectionAssertion collectionAssertion, IBatchRegister flowRegistry, IBufferLogger bufferLogger, ICurrencyUpdateService currencyUpdateService)
        {
            IDispatchMany<CurrencyUpdateResponse> updateResponseDispatcher = new ManagedDispatcher<CurrencyUpdateResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);

            IBatchMediator<CurrencyUpdate> updateMediator = new CurrencyUpdateMediator(currencyUpdateService, updateResponseDispatcher, collectionAssertion);
            IBatchController<CurrencyUpdate> updateController = new ManagedBatchController<CurrencyUpdate>(updateMediator);
            
            IErrorFactory<CurrencyUpdateError, IReadOnlyList<CurrencyUpdate>> updateErrorFactory = new CurrencyUpdateErrorFactory(baseErrorFactory);
            
            flowRegistry.RegisterBatch(updateController, updateErrorFactory);
        }
    }
}