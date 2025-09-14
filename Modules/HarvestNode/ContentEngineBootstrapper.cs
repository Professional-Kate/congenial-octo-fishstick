using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Factory;
using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Flows.Registry;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Logging;
using IdelPog.Core.Logging.Writer;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Controller;
using IdelPog.Core.Messaging.Dispatcher;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Core.Progression;
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

namespace IdelPog.HarvestNode
{
    public static class ContentEngineBootstrapper
    {
        /// <summary>
        /// Creates and adds the <see cref="SetHarvestNode"/> and <see cref="SkillUpdateResponse"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="currentHarvestTargetProvider">Used together with <see cref="ICurrentHarvestTargetProvider"/></param>
        /// <param name="batchRegister">Used to register the NodeCreation flow</param>
        /// <param name="singleRegister">Used to register the SkillUpdateResponse and SetHarvestNode flows</param>
        /// <seealso cref="RegisterSetHarvestNode"/>
        public static void RegisterFlows(IBufferManager bufferManager, CurrentHarvestTargetProvider currentHarvestTargetProvider, IBatchRegister batchRegister, ISingleRegister singleRegister)
        {
            IFoundAssertion foundAssertion = new FoundAssertion(new ThrowHandler());
            
            IAssetRepository<SkillID, SkillNodeEntity> skillNodeRepository = new AssetRepository<SkillID, SkillNodeEntity>();
            ISkillNodeAccessValidator skillNodeAccessValidator = new SkillNodeAccessValidator(skillNodeRepository, foundAssertion);
            
            ILogWriter writer = new ConsoleWriter();
            ILogger logger = new LoggingService(writer);
            
            RegisterSkillUpdateResponse(bufferManager, currentHarvestTargetProvider, skillNodeAccessValidator, singleRegister, logger);
            RegisterSetHarvestNode(bufferManager, currentHarvestTargetProvider, skillNodeAccessValidator, singleRegister, logger);
            RegisterNodeCreation(bufferManager, skillNodeRepository, batchRegister, logger);
        }

        /// <summary>
        /// Registers the <see cref="SkillUpdateResponse"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="currentHarvestTargetProvider">Used together with <see cref="ICurrentHarvestTargetSetter"/></param>
        /// <param name="skillNodeAccessValidator">Used to validate if a skill can access a node</param>
        /// <param name="singleRegister">Used to register the SkillUpdateResponse flow</param>
        /// <param name="logger">Logs all messages in and out</param>
        /// <remarks>
        /// Listens to -> <see cref="SkillUpdateResponse"/>. On Success -> <see cref="HarvestNodeUpdateResponse"/>. On Error -> <see cref="HarvestNodeUpdateError"/>
        /// </remarks>
        private static void RegisterSkillUpdateResponse(IBufferManager bufferManager, ICurrentHarvestTargetProvider currentHarvestTargetProvider, ISkillNodeAccessValidator skillNodeAccessValidator, ISingleRegister singleRegister, ILogger logger)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            ILevelAssertion levelAssertion = new LevelAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            ILevelableAssertionPipeline levelableAssertion = new LevelableAssertionPipeline(levelAssertion, objectNullAssertion);
            
            IAssetRepository<ItemID, ILootTable> lootTableRepository = new AssetRepository<ItemID, ILootTable>();
            IStateRepository<ItemID, Contracts.HarvestNode> harvestNodeRepository = new StateRepository<ItemID, Contracts.HarvestNode>();
            ILevelService levelService = new LevelService(levelableAssertion);
            IExperienceService experienceService = new ExperienceService(levelableAssertion);
            ILevelProgressFactory levelProgressFactory = new LevelProgressFactory();
            INodeUpdateResponseFactory responseFactory = new NodeUpdateResponseFactory(levelProgressFactory);
            
            Contracts.HarvestNode ironHarvestNode = new()
            {
                Information = new Information { Description = "", Name = "" }, 
                ItemID = ItemID.IRON, 
                Levelable = new Levelable(0, 0, 20, 0)
            };

            harvestNodeRepository.Add(ironHarvestNode.ItemID, ironHarvestNode);

            lootTableRepository.Add(ItemID.STONE, new GrantTable { ItemID = ItemID.STONE});
            lootTableRepository.Add(ItemID.COPPER, new GrantTable { ItemID = ItemID.COPPER});
            lootTableRepository.Add(ItemID.GOLD, new GrantTable { ItemID = ItemID.GOLD});
            lootTableRepository.Add(ItemID.IRON, new GrantTable { ItemID = ItemID.IRON});
            
            IDispatchOne<InventoryUpdate> inventoryUpdateDispatcher = new ManagedDispatcher<InventoryUpdate>(bufferManager, logger, objectNullAssertion, collectionAssertion);
            ILootService<ItemID> lootService = new LootService<ItemID>(lootTableRepository, inventoryUpdateDispatcher, new GrantPolicy(), foundAssertion);
            
            IDispatchOne<HarvestNodeUpdateResponse> responseDispatcher = new ManagedDispatcher<HarvestNodeUpdateResponse>(bufferManager, logger, objectNullAssertion, collectionAssertion);
            INodeUpdateService nodeUpdateService = new NodeUpdateService(harvestNodeRepository, levelService, experienceService, responseFactory, foundAssertion);
            ISingleMediator<SkillUpdateResponse> updateMediator = new NodeUpdateMediator(currentHarvestTargetProvider, skillNodeAccessValidator, nodeUpdateService, responseDispatcher, lootService);
            ISingleController<SkillUpdateResponse> nodeUpdateController = new ManagedSingleController<SkillUpdateResponse>(updateMediator);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<HarvestNodeUpdateError, SkillUpdateResponse> harvestNodeErrorFactory = new HarvestNodeUpdateErrorFactory(baseErrorFactory);

            singleRegister.Register(nodeUpdateController, harvestNodeErrorFactory);
        }

        /// <summary>
        /// Registers the <see cref="SetHarvestNode"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="currentHarvestTargetSetter">Used together with <see cref="ICurrentHarvestTargetProvider"/></param>
        /// <param name="skillNodeAccessValidator">Used to validate if a skill can access a node</param>
        /// <param name="singleRegister">Used to register the SetHarvestNode flow</param>
        /// <param name="logger">Logs all messages in and out</param>
        /// <remarks>
        /// Listens to -> <see cref="SetHarvestNode"/>. On Success -> <see cref="SetHarvestNodeResponse"/>. On Error -> <see cref="SetHarvestNodeError"/>
        /// </remarks>
        private static void RegisterSetHarvestNode(IBufferManager bufferManager, ICurrentHarvestTargetSetter currentHarvestTargetSetter, ISkillNodeAccessValidator skillNodeAccessValidator, ISingleRegister singleRegister, ILogger logger)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<SetHarvestNodeError, SetHarvestNode> setHarvestNodeErrorFactory = new SetNodeErrorFactory(baseErrorFactory);

            IDispatchOne<SetHarvestNodeResponse> setHarvestNodeResponseDispatcher = new ManagedDispatcher<SetHarvestNodeResponse>(bufferManager, logger, objectNullAssertion, collectionAssertion);
            ISetNodeResponseFactory nodeChangeResponseFactor = new SetNodeResponseFactory();
            ISingleMediator<SetHarvestNode> setHarvestNodeMediator = new SetHarvestNodeMediator(skillNodeAccessValidator, currentHarvestTargetSetter, setHarvestNodeResponseDispatcher, nodeChangeResponseFactor);
            ISingleController<SetHarvestNode> setHarvestNodeController = new ManagedSingleController<SetHarvestNode>(setHarvestNodeMediator);
            
            singleRegister.Register(setHarvestNodeController, setHarvestNodeErrorFactory);
        }

        /// <summary>
        /// Registers the <see cref="NodeCreation"/> flow into the messaging system>
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="skillNodeRepository">Used to store all <see cref="HarvestNode"/> models</param>
        /// <param name="batchRegister">Used to register the NodeCreation flow</param>
        /// <param name="logger">Logs all messages in and out</param>
        /// <remarks>
        /// Listens to -> <see cref="NodeCreation"/>. On Success -> <see cref="NodeCreationResponse"/>. On Error -> <see cref="NodeCreationError"/>
        /// </remarks>
        private static void RegisterNodeCreation(IBufferManager bufferManager, IAssetRepository<SkillID, SkillNodeEntity> skillNodeRepository, IBatchRegister batchRegister, ILogger logger)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);

            IStateRepository<ItemID, Contracts.HarvestNode> harvestNodeRepository = new StateRepository<ItemID, Contracts.HarvestNode>();
            ISkillNodeEntityFactory skillNodeEntityFactory = new SkillNodeEntityFactory();
            IHarvestNodeFactory harvestNodeFactory = new HarvestNodeFactory();
            INodeCreationResponseFactory nodeCreationResponseFactory = new NodeCreationResponseFactory();
            IDispatchOne<NodeCreationResponse> nodeCreationResponseDispatcher = new ManagedDispatcher<NodeCreationResponse>(bufferManager, logger, objectNullAssertion, collectionAssertion);
            
            IBatchMediator<NodeCreation> creationMediator = new NodeCreationMediator(harvestNodeRepository, skillNodeRepository, skillNodeEntityFactory, harvestNodeFactory, nodeCreationResponseFactory, nodeCreationResponseDispatcher, uniqueAssertion, collectionAssertion);
            IBatchController<NodeCreation> creationController = new ManagedBatchController<NodeCreation>(creationMediator);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<NodeCreationError, IReadOnlyList<NodeCreation>> errorFactory = new NodeCreationErrorFactory(baseErrorFactory);

            batchRegister.Register(creationController, errorFactory);
        }
    }
}