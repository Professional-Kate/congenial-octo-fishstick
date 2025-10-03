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
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Core.Progression.Assertion;
using IdelPog.Core.Progression.Assertion.Pipelines;
using IdelPog.Core.Progression.Experience;
using IdelPog.Core.Progression.Level;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.HarvestNode.Contracts;
using IdelPog.HarvestNode.Factory;
using IdelPog.HarvestNode.Factory.Interface;
using IdelPog.HarvestNode.Runtime.ECS;
using IdelPog.HarvestNode.Runtime.Factory;
using IdelPog.HarvestNode.Runtime.Factory.Interfaces;
using IdelPog.HarvestNode.Runtime.Mediator;
using IdelPog.HarvestNode.Runtime.System;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.HarvestNode.Services;
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
        /// <param name="singleRegister">Used to register the SkillUpdateResponse and SetHarvestNode flows</param>
        /// <seealso cref="RegisterSetHarvestNode"/>
        public static void RegisterFlows(IBufferManager bufferManager, IBatchRegister batchRegister, ISingleRegister singleRegister)
        {
            IFoundAssertion foundAssertion = new FoundAssertion(new ThrowHandler());
            
            IAssetRepository<SkillID, SkillNodeEntity> skillNodeRepository = new AssetRepository<SkillID, SkillNodeEntity>();
            ISkillNodeAccessValidator skillNodeAccessValidator = new SkillNodeAccessValidator(skillNodeRepository, foundAssertion);
            
            ILogWriter writer = new ConsoleWriter();
            IBufferLogger bufferLogger = new BufferLoggingService(writer);
            CurrentHarvestTargetProvider currentHarvestTargetProvider = new();
            IStateRepository<ItemID, Contracts.HarvestNode> harvestNodeRepository = new StateRepository<ItemID, Contracts.HarvestNode>();
            IAssetRepository<SkillID, UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>> entityRepository = new AssetRepository<SkillID, UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>>();
            
            
            RegisterSkillUpdateResponse(bufferManager, currentHarvestTargetProvider, skillNodeAccessValidator, singleRegister, bufferLogger, harvestNodeRepository);
            RegisterSetHarvestNode(bufferManager, currentHarvestTargetProvider, skillNodeAccessValidator, singleRegister, bufferLogger);
            RegisterNodeCreation(bufferManager, skillNodeRepository, batchRegister, bufferLogger, harvestNodeRepository);
            RegisterNodeUnlock(bufferManager, batchRegister, bufferLogger, entityRepository);
            RegisterNodeRequirementsCreation(bufferManager, batchRegister, bufferLogger, entityRepository);
        }

        /// <summary>
        /// Registers the <see cref="SkillUpdateResponse"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="currentHarvestTargetProvider">Used together with <see cref="ICurrentHarvestTargetSetter"/></param>
        /// <param name="skillNodeAccessValidator">Used to validate if a skill can access a node</param>
        /// <param name="singleRegister">Used to register the SkillUpdateResponse flow</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <param name="harvestNodeRepository">Stores all HarvestNodes</param>
        /// <remarks>
        /// Listens to -> <see cref="SkillUpdateResponse"/>. On Success -> <see cref="HarvestNodeUpdateResponse"/>. On Error -> <see cref="HarvestNodeUpdateError"/>
        /// </remarks>
        private static void RegisterSkillUpdateResponse(IBufferManager bufferManager, ICurrentHarvestTargetProvider currentHarvestTargetProvider, ISkillNodeAccessValidator skillNodeAccessValidator, ISingleRegister singleRegister, IBufferLogger bufferLogger, IStateRepository<ItemID, Contracts.HarvestNode> harvestNodeRepository)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            ILevelAssertion levelAssertion = new LevelAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            ILevelableAssertionPipeline levelableAssertion = new LevelableAssertionPipeline(levelAssertion, objectNullAssertion);
            
            IAssetRepository<ItemID, ILootTable> lootTableRepository = new AssetRepository<ItemID, ILootTable>();
            ILevelService levelService = new LevelService(levelableAssertion);
            IExperienceService experienceService = new ExperienceService(levelableAssertion);
            ILevelProgressFactory levelProgressFactory = new LevelProgressFactory();
            INodeUpdateResponseFactory responseFactory = new NodeUpdateResponseFactory(levelProgressFactory);
            
            LootTableGrantSelf(lootTableRepository, [ItemID.STONE, ItemID.IRON, ItemID.COPPER, ItemID.GOLD, ItemID.OAK, ItemID.SPRUCE, ItemID.BIRCH, ItemID.HERBS, ItemID.SMALL_INSECTS, ItemID.HONEY, ItemID.WATER, ItemID.SAND]);
            
            IDispatchOne<InventoryUpdate> inventoryUpdateDispatcher = new ManagedDispatcher<InventoryUpdate>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            ILootService<ItemID> lootService = new LootService<ItemID>(lootTableRepository, inventoryUpdateDispatcher, new GrantPolicy(), foundAssertion);
            
            IDispatchOne<HarvestNodeUpdateResponse> responseDispatcher = new ManagedDispatcher<HarvestNodeUpdateResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            INodeUpdateService nodeUpdateService = new NodeUpdateService(harvestNodeRepository, levelService, experienceService, responseFactory, foundAssertion);
            ISingleMediator<SkillUpdateResponse> updateMediator = new NodeUpdateMediator(currentHarvestTargetProvider, skillNodeAccessValidator, nodeUpdateService, responseDispatcher, lootService);
            ISingleController<SkillUpdateResponse> nodeUpdateController = new ManagedSingleController<SkillUpdateResponse>(updateMediator);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<HarvestNodeUpdateError, SkillUpdateResponse> harvestNodeErrorFactory = new HarvestNodeUpdateErrorFactory(baseErrorFactory);

            singleRegister.RegisterSingle(nodeUpdateController, harvestNodeErrorFactory);
        }

        private static void LootTableGrantSelf(IAssetRepository<ItemID, ILootTable> lootTableRepository, ItemID[] itemIDs)
        {
            foreach (ItemID itemID in itemIDs)
            {
                lootTableRepository.Add(itemID, new GrantTable { ItemID = itemID });
            }
        }

        /// <summary>
        /// Registers the <see cref="SetHarvestNode"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="currentHarvestTargetSetter">Used together with <see cref="ICurrentHarvestTargetProvider"/></param>
        /// <param name="skillNodeAccessValidator">Used to validate if a skill can access a node</param>
        /// <param name="singleRegister">Used to register the SetHarvestNode flow</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <remarks>
        /// Listens to -> <see cref="SetHarvestNode"/>. On Success -> <see cref="SetHarvestNodeResponse"/>. On Error -> <see cref="SetHarvestNodeError"/>
        /// </remarks>
        private static void RegisterSetHarvestNode(IBufferManager bufferManager, ICurrentHarvestTargetSetter currentHarvestTargetSetter, ISkillNodeAccessValidator skillNodeAccessValidator, ISingleRegister singleRegister, IBufferLogger bufferLogger)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<SetHarvestNodeError, SetHarvestNode> setHarvestNodeErrorFactory = new SetNodeErrorFactory(baseErrorFactory);

            IDispatchOne<SetHarvestNodeResponse> setHarvestNodeResponseDispatcher = new ManagedDispatcher<SetHarvestNodeResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            ISetNodeResponseFactory nodeChangeResponseFactor = new SetNodeResponseFactory();
            ISingleMediator<SetHarvestNode> setHarvestNodeMediator = new SetHarvestNodeMediator(skillNodeAccessValidator, currentHarvestTargetSetter, setHarvestNodeResponseDispatcher, nodeChangeResponseFactor);
            ISingleController<SetHarvestNode> setHarvestNodeController = new ManagedSingleController<SetHarvestNode>(setHarvestNodeMediator);
            
            singleRegister.RegisterSingle(setHarvestNodeController, setHarvestNodeErrorFactory);
        }

        /// <summary>
        /// Registers the <see cref="NodeCreation"/> flow into the messaging system>
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="skillNodeRepository">Used to store all <see cref="HarvestNode"/> models</param>
        /// <param name="batchRegister">Used to register the NodeCreation flow</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <param name="harvestNodeRepository">Stores all HarvestNodes</param>
        /// <remarks>
        /// Listens to -> <see cref="NodeCreation"/>. On Success -> <see cref="NodeCreationResponse"/>. On Error -> <see cref="NodeCreationError"/>
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
            IDispatchOne<NodeCreationResponse> nodeCreationResponseDispatcher = new ManagedDispatcher<NodeCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            IBatchMediator<NodeCreation> creationMediator = new NodeCreationMediator(harvestNodeRepository, skillNodeRepository, skillNodeEntityFactory, harvestNodeFactory, nodeCreationResponseFactory, nodeCreationResponseDispatcher, uniqueAssertion, collectionAssertion);
            IBatchController<NodeCreation> creationController = new ManagedBatchController<NodeCreation>(creationMediator);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<NodeCreationError, IReadOnlyList<NodeCreation>> errorFactory = new NodeCreationErrorFactory(baseErrorFactory);

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
            ISkillMatchesAssertion<SkillID> skillMatchesAssertion = new SkillMatchesAssertion<SkillID>(throwHandler);
            IQueueAssertion<SkillID, HarvestNodeUnlockResponse> queueAssertion = new QueueAssertion<SkillID, HarvestNodeUnlockResponse>(throwHandler);

            IEntityUnlockerService<SkillID, HarvestNodeUnlockResponse> entityUnlockerService = new EntityUnlockerService<SkillID, HarvestNodeUnlockResponse>(entityRepository, foundAssertion, canUnlockAssertion, skillMatchesAssertion, queueAssertion);
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
    }
}