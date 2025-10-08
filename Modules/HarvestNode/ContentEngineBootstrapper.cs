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
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Progression.Assertion;
using IdelPog.Core.Progression.Experience;
using IdelPog.Core.Progression.Level;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.HarvestNode.Assertion;
using IdelPog.HarvestNode.Assertion.Interface;
using IdelPog.HarvestNode.Contracts;
using IdelPog.HarvestNode.Factory;
using IdelPog.HarvestNode.Factory.Interface;
using IdelPog.HarvestNode.Runtime.ECS;
using IdelPog.HarvestNode.Runtime.Factory;
using IdelPog.HarvestNode.Runtime.Factory.Interfaces;
using IdelPog.HarvestNode.Runtime.Mediator;
using IdelPog.HarvestNode.Runtime.System;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.Loot.Assertion;
using IdelPog.Loot.Assertion.Interface;
using IdelPog.Loot.Policy;
using IdelPog.Loot.Service;
using IdelPog.Loot.Service.Interface;
using IdelPog.Loot.Table;
using IdelPog.Progression.Assertion;
using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Runtime;
using IdelPog.Progression.Runtime.System;
using IdelPog.Progression.Runtime.System.Interface;

namespace IdelPog.HarvestNode
{
    public static class ContentEngineBootstrapper
    {
        /// <summary>
        /// Registers all flows
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="batchRegister">Used to register the NodeCreation flow</param>
        public static void RegisterFlows(IBufferManager bufferManager, IBatchRegister batchRegister)
        {
            IFoundAssertion foundAssertion = new FoundAssertion(new ThrowHandler());
            
            IAssetRepository<SkillID, SkillNodeEntity> skillNodeRepository = new AssetRepository<SkillID, SkillNodeEntity>();
            ISkillNodeAccessValidator skillNodeAccessValidator = new SkillNodeAccessValidator(skillNodeRepository, foundAssertion);
            
            ILogWriter writer = new ConsoleWriter();
            IBufferLogger bufferLogger = new BufferLoggingService(writer);
            IStateRepository<ItemID, Contracts.HarvestNode> harvestNodeRepository = new StateRepository<ItemID, Contracts.HarvestNode>();
            IAssetRepository<SkillID, UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>> entityRepository = new AssetRepository<SkillID, UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>>();
            IAssetRepository<ItemID, ILootTable> itemLootTableRepository = new AssetRepository<ItemID, ILootTable>();
            
            RegisterSkillUpdateResponse(bufferManager, skillNodeAccessValidator, batchRegister, bufferLogger, harvestNodeRepository, entityRepository, itemLootTableRepository);
            RegisterNodeCreation(bufferManager, skillNodeRepository, batchRegister, bufferLogger, harvestNodeRepository);
            RegisterNodeUnlock(bufferManager, batchRegister, bufferLogger, entityRepository);
            RegisterNodeRequirementsCreation(bufferManager, batchRegister, bufferLogger, entityRepository);
            RegisterHarvestNodeLootCreation(bufferManager, batchRegister, bufferLogger, itemLootTableRepository);
        }

        /// <summary>
        /// Registers the <see cref="SkillUpdateResponse"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="skillNodeAccessValidator">Used to validate if a skill can access a node</param>
        /// <param name="batchRegister">Used to register the SkillUpdateResponse flow</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <param name="harvestNodeRepository">Stores all HarvestNodes</param>
        /// <param name="entityRepository">Stores all <see cref="UnlockRequirementsEntity{TID,TCommand}"/></param>
        /// <param name="itemLootTableRepository">Stores all <see cref="ILootTable"/></param>
        /// <remarks>
        /// Listens to -> <see cref="SkillUpdateResponse"/>. On Success -> <see cref="HarvestNodeUpdateResponse"/>. On Error -> <see cref="HarvestNodeUpdateError"/>
        /// </remarks>
        private static void RegisterSkillUpdateResponse(IBufferManager bufferManager, ISkillNodeAccessValidator skillNodeAccessValidator, IBatchRegister batchRegister, IBufferLogger bufferLogger, IStateRepository<ItemID, Contracts.HarvestNode> harvestNodeRepository, IAssetRepository<SkillID, UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>> entityRepository, IAssetRepository<ItemID, ILootTable> itemLootTableRepository)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            ILevelAssertion levelAssertion = new LevelAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            INodeUnlockedAssertion nodeUnlockedAssertion = new NodeUnlockedAssertion(throwHandler);
            
            IAssetRepository<LocationID, ILootTable> locationLootTableRepository = new AssetRepository<LocationID, ILootTable>();
            ILevelService levelService = new LevelService(levelAssertion, objectNullAssertion);
            IExperienceService experienceService = new ExperienceService(levelAssertion, objectNullAssertion);
            ILevelProgressFactory levelProgressFactory = new LevelProgressFactory();
            INodeUpdateResponseFactory responseFactory = new NodeUpdateResponseFactory(levelProgressFactory);
            IEntityUnlockChecker<SkillID, HarvestNodeUnlockResponse> unlockChecker = new EntityUnlockChecker<SkillID, HarvestNodeUnlockResponse>(entityRepository);
            
            IDispatchMany<InventoryUpdate> inventoryUpdateDispatcher = new ManagedDispatcher<InventoryUpdate>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            ILootService<ItemID> itemLootService = new LootService<ItemID>(itemLootTableRepository, new GrantPolicy(), foundAssertion);
            ILootService<LocationID> locationLootService = new LootService<LocationID>(locationLootTableRepository, new GrantPolicy(), foundAssertion);
            IHarvestNodeLootService harvestNodeLootService = new HarvestNodeLootService(itemLootService, locationLootService);
            
            IDispatchMany<HarvestNodeUpdateResponse> responseDispatcher = new ManagedDispatcher<HarvestNodeUpdateResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            INodeUpdateService nodeUpdateService = new NodeUpdateService(harvestNodeRepository, levelService, experienceService, responseFactory, foundAssertion);
            IBatchMediator<HarvestNodeUpdate> updateMediator = new NodeUpdateMediator(harvestNodeRepository, skillNodeAccessValidator, unlockChecker, nodeUpdateService, harvestNodeLootService, responseDispatcher, inventoryUpdateDispatcher, nodeUnlockedAssertion, collectionAssertion, foundAssertion);
            IBatchController<HarvestNodeUpdate> nodeUpdateController = new ManagedBatchController<HarvestNodeUpdate>(updateMediator);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<HarvestNodeUpdateError, IReadOnlyList<HarvestNodeUpdate>> harvestNodeErrorFactory = new HarvestNodeUpdateErrorFactory(baseErrorFactory);

            batchRegister.RegisterBatch(nodeUpdateController, harvestNodeErrorFactory);
        }

        /// <summary>
        /// Registers the <see cref="HarvestNodeCreation"/> flow into the messaging system>
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="skillNodeRepository">Used to store all <see cref="HarvestNode"/> models</param>
        /// <param name="batchRegister">Used to register the NodeCreation flow</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <param name="harvestNodeRepository">Stores all HarvestNodes</param>
        /// <remarks>
        /// Listens to -> <see cref="HarvestNodeCreation"/>. On Success -> <see cref="HarvestNodeCreationResponse"/>. On Error -> <see cref="HarvestNodeCreationError"/>
        /// </remarks>
        private static void RegisterNodeCreation(IBufferManager bufferManager, IAssetRepository<SkillID, SkillNodeEntity> skillNodeRepository, IBatchRegister batchRegister, IBufferLogger bufferLogger, IStateRepository<ItemID, Contracts.HarvestNode> harvestNodeRepository)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);

            ISkillNodeEntityFactory skillNodeEntityFactory = new SkillNodeEntityFactory();
            IHarvestNodeFactory harvestNodeFactory = new HarvestNodeFactory();
            INodeCreationResponseFactory nodeCreationResponseFactory = new NodeCreationResponseFactory();
            IDispatchMany<HarvestNodeCreationResponse> nodeCreationResponseDispatcher = new ManagedDispatcher<HarvestNodeCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            IBatchMediator<HarvestNodeCreation> creationMediator = new NodeCreationMediator(harvestNodeRepository, skillNodeRepository, skillNodeEntityFactory, harvestNodeFactory, nodeCreationResponseFactory, nodeCreationResponseDispatcher, uniqueAssertion, collectionAssertion);
            IBatchController<HarvestNodeCreation> creationController = new ManagedBatchController<HarvestNodeCreation>(creationMediator);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<HarvestNodeCreationError, IReadOnlyList<HarvestNodeCreation>> errorFactory = new NodeCreationErrorFactory(baseErrorFactory);

            batchRegister.RegisterBatch(creationController, errorFactory);
        }

        /// <summary>
        /// Registers the <see cref="HarvestNodeUnlock"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="batchRegister">Used to register the NodeCreation flow</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <param name="entityRepository">Stores all <see cref="UnlockRequirementsEntity{TID,TCommand}"/></param>
        /// /// <remarks>
        /// Listens to -> <see cref="HarvestNodeUnlock"/>. On Success -> <see cref="HarvestNodeUnlockResponse"/>. On Error -> <see cref="HarvestNodeUnlockError"/>
        /// </remarks>
        private static void RegisterNodeUnlock(IBufferManager bufferManager, IBatchRegister batchRegister, IBufferLogger bufferLogger, IAssetRepository<SkillID, UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>> entityRepository)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            ICanUnlockAssertion<SkillID, HarvestNodeUnlockResponse> canUnlockAssertion = new CanUnlockAssertion<SkillID, HarvestNodeUnlockResponse>(throwHandler);
            IIDMatchesAssertion<SkillID> iidMatchesAssertion = new IDMatchesAssertion<SkillID>(throwHandler);
            IQueueAssertion<SkillID, HarvestNodeUnlockResponse> queueAssertion = new QueueAssertion<SkillID, HarvestNodeUnlockResponse>(throwHandler);

            IEntityUnlockerService<SkillID, HarvestNodeUnlockResponse> entityUnlockerService = new EntityUnlockerService<SkillID, HarvestNodeUnlockResponse>(entityRepository, foundAssertion, canUnlockAssertion, iidMatchesAssertion, queueAssertion);
            IDispatchMany<HarvestNodeUnlockResponse> responseDispatcher = new ManagedDispatcher<HarvestNodeUnlockResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            IBatchMediator<HarvestNodeUnlock> unlockMediator = new NodeUnlockMediator(entityUnlockerService, responseDispatcher, collectionAssertion);
            IBatchController<HarvestNodeUnlock> unlockController = new ManagedBatchController<HarvestNodeUnlock>(unlockMediator);
            
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<HarvestNodeUnlockError, IReadOnlyList<HarvestNodeUnlock>> errorFactory = new NodeUnlockErrorFactory(baseErrorFactory);
            
            batchRegister.RegisterBatch(unlockController, errorFactory);
        }

        /// <summary>
        /// Registers the <see cref="HarvestNodeUnlock"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="batchRegister">Used to register the NodeCreation flow</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <param name="entityRepository">Stores all <see cref="UnlockRequirementsEntity{TID,TCommand}"/></param>
        /// /// <remarks>
        /// Listens to -> <see cref="HarvestNodeRequirementsCreation"/>. On Success -> <see cref="HarvestNodeRequirementsCreationResponse"/>. On Error -> <see cref="HarvestNodeRequirementsCreationError"/>
        /// </remarks>
        private static void RegisterNodeRequirementsCreation(IBufferManager bufferManager, IBatchRegister batchRegister, IBufferLogger bufferLogger, IAssetRepository<SkillID, UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>> entityRepository)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);
            
            IDispatchMany<HarvestNodeRequirementsCreationResponse> responseDispatcher = new ManagedDispatcher<HarvestNodeRequirementsCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            IUnlockRequirementsEntityFactory entityFactory = new UnlockRequirementsEntityFactory();
                
            IBatchMediator<HarvestNodeRequirementsCreation> creationMediator = new NodeRequirementsCreationMediator(entityRepository, entityFactory, responseDispatcher, collectionAssertion, uniqueAssertion);
            IBatchController<HarvestNodeRequirementsCreation> creationController = new ManagedBatchController<HarvestNodeRequirementsCreation>(creationMediator);
            
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<HarvestNodeRequirementsCreationError, IReadOnlyList<HarvestNodeRequirementsCreation>> errorFactory = new NodeRequirementsCreationErrorFactory(baseErrorFactory);
            
            batchRegister.RegisterBatch(creationController, errorFactory);
        }

        /// <summary>
        /// Registers the <see cref="HarvestNodeLootCreation"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="batchRegister">Used to register the NodeCreation flow</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <param name="itemLootTableRepository">Stores all <see cref="ILootTable"/></param>
        /// <remarks>
        /// Listens to -> <see cref="HarvestNodeLootCreation"/>. On Success -> <see cref="HarvestNodeLootCreationResponse"/>. On Error -> <see cref="HarvestNodeLootCreationError"/>
        /// </remarks>
        private static void RegisterHarvestNodeLootCreation(IBufferManager bufferManager, IBatchRegister batchRegister, IBufferLogger bufferLogger, IAssetRepository<ItemID, ILootTable> itemLootTableRepository)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);
            IWeightAssertion weightAssertion = new WeightAssertion(throwHandler);
            
            IWeightedLootTableFactory lootTableFactory = new WeightedLootTableFactory(collectionAssertion, weightAssertion);
            IDispatchMany<HarvestNodeLootCreationResponse> responseDispatcher = new ManagedDispatcher<HarvestNodeLootCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            IBatchMediator<HarvestNodeLootCreation> creationMediator = new NodeLootGenerationMediator(itemLootTableRepository, lootTableFactory, responseDispatcher, collectionAssertion, uniqueAssertion);
            IBatchController<HarvestNodeLootCreation> creationController = new ManagedBatchController<HarvestNodeLootCreation>(creationMediator);
            
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            NodeLootCreationErrorFactory errorFactory = new(baseErrorFactory);
            
            batchRegister.RegisterBatch(creationController, errorFactory);
        }
    }
}