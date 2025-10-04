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
using IdelPog.Core.Progression.Assertion;
using IdelPog.Core.Progression.Experience;
using IdelPog.Core.Progression.Level;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Loot.Assertion;
using IdelPog.Loot.Assertion.Interface;
using IdelPog.Loot.Policy;
using IdelPog.Loot.Random;
using IdelPog.Loot.Service;
using IdelPog.Loot.Service.Interface;
using IdelPog.Loot.Table;
using IdelPog.Skill.Factory;
using IdelPog.Skill.Factory.Interface;
using IdelPog.Skill.Mediator;

namespace IdelPog.Skill
{
    public static class SkillBootstrapper
    {
        public static void RegisterFlows(IBufferManager bufferManager, FlowRegister flowRegistry)
        {
            ILogWriter writer = new ConsoleWriter();
            IBufferLogger bufferLogger = new BufferLoggingService(writer);
            
            IStateRepository<SkillID, Contracts.Skill> skillRepository = new StateRepository<SkillID, Contracts.Skill>();
            
            RegisterSkillUpdate(bufferManager, flowRegistry, skillRepository, bufferLogger);
            RegisterSkillCreation(bufferManager, flowRegistry, skillRepository, bufferLogger);
        }

        /// <summary>
        /// Registers the <see cref="SkillCreation"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="SkillCreationResponse"/></param>
        /// <param name="flowRegistry">Used to register the SetSkill flow</param>
        /// <param name="skillRepository">Used to store Skills</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// /// <remarks>
        /// Listens to -> <see cref="SkillCreation"/>. On Success -> <see cref="SkillCreationResponse"/>. On Error -> <see cref="SkillCreationError"/>
        /// </remarks>
        private static void RegisterSkillCreation(IBufferManager bufferManager, IBatchRegister flowRegistry, IStateRepository<SkillID, Contracts.Skill> skillRepository, IBufferLogger bufferLogger)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);
            ILevelAssertion levelAssertion = new LevelAssertion(throwHandler);
            
            ISkillCreationResponseFactory responseFactory = new SkillCreationResponseFactory();
            IDispatchMany<SkillCreationResponse> responseDispatcher = new ManagedDispatcher<SkillCreationResponse>(bufferManager, bufferLogger,  objectNullAssertion, collectionAssertion);
                
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<SkillCreationError, IReadOnlyList<SkillCreation>> errorFactory = new SkillCreationErrorFactory(baseErrorFactory);
            
            IBatchMediator<SkillCreation> mediator = new SkillCreationMediator(skillRepository, responseFactory, responseDispatcher, collectionAssertion, uniqueAssertion, levelAssertion);
            IBatchController<SkillCreation> controller = new ManagedBatchController<SkillCreation>(mediator);
            
            flowRegistry.RegisterBatch(controller, errorFactory);
        }

        /// <summary>
        /// Registers the <see cref="SkillUpdate"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="SkillUpdateResponse"/></param>
        /// <param name="flowRegistry">Used to register the ScheduleTick flow</param>
        /// <param name="skillRepository">Used to store Skills</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// /// <remarks>
        /// Listens to -> <see cref="SkillUpdate"/>. On Success -> <see cref="SkillUpdateResponse"/>. On Error -> <see cref="SkillUpdateError"/>.
        /// </remarks>
        private static void RegisterSkillUpdate(IBufferManager bufferManager, IBatchRegister flowRegistry, IStateRepository<SkillID, Contracts.Skill> skillRepository, IBufferLogger bufferLogger)
        {
            IHandler throwHandler = new ThrowHandler();
            ILevelAssertion levelAssertion = new LevelAssertion(throwHandler);
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            IWeightAssertion weightAssertion = new WeightAssertion(throwHandler);
            
            ILevelProgressFactory levelProgressFactory = new LevelProgressFactory();
            
            IExperienceService experienceService = new ExperienceService(levelAssertion, objectNullAssertion);
            ILevelService levelService = new LevelService(levelAssertion, objectNullAssertion);
            IDispatchMany<SkillUpdateResponse> responseDispatcher = new ManagedDispatcher<SkillUpdateResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            ISkillUpdateResponseFactory updateResponseFactory = new SkillUpdateResponseFactory(levelProgressFactory);

            ILootRoll lootRoll = new DefaultLootRoll();
            IDispatchOne<InventoryUpdate> inventoryUpdateDispatcher = new ManagedDispatcher<InventoryUpdate>(bufferManager, bufferLogger,  objectNullAssertion, collectionAssertion);
            IAssetRepository<SkillID, ILootTable> weightedLootTableRepository = new AssetRepository<SkillID, ILootTable>();
            IGrantPolicy grantPolicy = new WeightedPolicy(lootRoll, grantWeight: 1, skipWeight: 100, weightAssertion);
            ILootService<SkillID> lootService = new LootService<SkillID>(weightedLootTableRepository, inventoryUpdateDispatcher, grantPolicy, foundAssertion);

            WeightedEntry[] miningEntries =
            [
                new()
                {
                    ItemID = ItemID.DIAMOND,
                    Weight = 1
                },
                new()
                {
                    ItemID = ItemID.EMERALD,
                    Weight = 3
                },
                new()
                {
                    ItemID = ItemID.RUBY,
                    Weight = 5
                }
            ];
            
            ILootTable miningLootTable = new WeightedLootTable(miningEntries, lootRoll, collectionAssertion, weightAssertion);
            weightedLootTableRepository.Add(SkillID.MINING, miningLootTable);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<SkillUpdateError, IReadOnlyList<SkillUpdate>> updateErrorFactory = new SkillUpdateErrorFactory(baseErrorFactory);
            IBatchMediator<SkillUpdate> skillActionMediator = new SkillUpdateMediator(experienceService, levelService, skillRepository, responseDispatcher, updateResponseFactory, lootService);
            IBatchController<SkillUpdate> skillActionController = new ManagedBatchController<SkillUpdate>(skillActionMediator);
            
            flowRegistry.RegisterBatch(skillActionController, updateErrorFactory);
        }
    }
}