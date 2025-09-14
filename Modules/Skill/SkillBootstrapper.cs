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
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Core.Progression.Assertion;
using IdelPog.Core.Progression.Assertion.Pipelines;
using IdelPog.Core.Progression.Experience;
using IdelPog.Core.Progression.Level;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Scheduler;
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
using IdelPog.Skill.Service;

namespace IdelPog.Skill
{
    public static class SkillBootstrapper
    {
        public static void RegisterFlows(IBufferManager bufferManager, ISingleRegister flowRegistry)
        {
            CurrentSkillProvider skillProvider = new();
            
            ILogWriter writer = new ConsoleWriter();
            ILogger logger = new LoggingService(writer);
            
            RegisterSetSkill(bufferManager, skillProvider, flowRegistry, logger);
            RegisterScheduleTick(bufferManager, skillProvider, flowRegistry, logger);
        }
        
        /// <summary>
        /// Registers the <see cref="SetSkill"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="SetSkillResponse"/></param>
        /// <param name="currentSkillSetter">Used together with <see cref="ICurrentSkillProvider"/></param>
        /// <param name="flowRegistry">Used to register the SetSkill flow</param>
        /// <param name="logger">Logs all messages in and out</param>
        /// <remarks>
        /// Listens to -> <see cref="SetSkill"/>. On Success -> <see cref="SetSkillResponse"/>. On Error -> <see cref="SetSkillError"/>
        /// </remarks>
        private static void RegisterSetSkill(IBufferManager bufferManager, ICurrentSkillSetter  currentSkillSetter, ISingleRegister flowRegistry, ILogger logger)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<SetSkillError, SetSkill> setSkillErrorFactory = new SetSkillErrorFactory(baseErrorFactory );

            ISetSkillResponseFactory setSkillResponseFactory = new SetSkillResponseFactory();
            IDispatchOne<SetSkillResponse> setSkillResponseDispatcher = new ManagedDispatcher<SetSkillResponse>(bufferManager, logger, objectNullAssertion, collectionAssertion);
            ISingleMediator<SetSkill> setSkillMediator = new SetSkillMediator(currentSkillSetter, setSkillResponseFactory, setSkillResponseDispatcher);
            ISingleController<SetSkill> setSkillController = new ManagedSingleController<SetSkill>(setSkillMediator);
            
            flowRegistry.Register(setSkillController, setSkillErrorFactory);
        }

        /// <summary>
        /// Registers the <see cref="ScheduleTick"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="SetSkillResponse"/></param>
        /// <param name="currentSkillProvider">Used together with <see cref="ICurrentSkillSetter"/></param>
        /// <param name="flowRegistry">Used to register the ScheduleTick flow</param>
        /// <param name="logger">Logs all messages in and out</param>
        /// /// <remarks>
        /// Listens to -> <see cref="ScheduleTick"/>. On Success -> <see cref="SkillUpdateResponse"/>. On Error -> <see cref="SkillUpdateError"/>.
        /// </remarks>
        private static void RegisterScheduleTick(IBufferManager bufferManager, ICurrentSkillProvider currentSkillProvider, ISingleRegister flowRegistry, ILogger logger)
        {
            IHandler throwHandler = new ThrowHandler();
            ILevelAssertion levelAssertion = new LevelAssertion(throwHandler);
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            IWeightAssertion weightAssertion = new WeightAssertion(throwHandler);
            
            ILevelableAssertionPipeline levelableAssertionPipeline = new LevelableAssertionPipeline(levelAssertion, objectNullAssertion);
            ILevelProgressFactory levelProgressFactory = new LevelProgressFactory();
            
            IExperienceService experienceService = new ExperienceService(levelableAssertionPipeline);
            ILevelService levelService = new LevelService(levelableAssertionPipeline);
            IStateRepository<SkillID, Contracts.Skill> skillRepository = new StateRepository<SkillID, Contracts.Skill>();
            IDispatchOne<SkillUpdateResponse> responseDispatcher = new ManagedDispatcher<SkillUpdateResponse>(bufferManager, logger, objectNullAssertion, collectionAssertion);
            ISkillUpdateResponseFactory updateResponseFactory = new SkillUpdateResponseFactory(levelProgressFactory);

            ILootRoll lootRoll = new DefaultLootRoll();
            IDispatchOne<InventoryUpdate> inventoryUpdateDispatcher = new ManagedDispatcher<InventoryUpdate>(bufferManager, logger,  objectNullAssertion, collectionAssertion);
            IAssetRepository<SkillID, ILootTable> weightedLootTableRepository = new AssetRepository<SkillID, ILootTable>();
            IGrantPolicy grantPolicy = new WeightedPolicy(lootRoll, grantWeight: 1, skipWeight: 10, weightAssertion);
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
            IErrorFactory<SkillUpdateError, ScheduleTick> updateErrorFactory = new SkillUpdateErrorFactory(baseErrorFactory);
            ISingleMediator<ScheduleTick> skillActionMediator = new SkillActionMediator(experienceService, levelService, skillRepository, currentSkillProvider, responseDispatcher, updateResponseFactory, lootService);
            ISingleController<ScheduleTick> skillActionController = new ManagedSingleController<ScheduleTick>(skillActionMediator);
            
            flowRegistry.Register(skillActionController, updateErrorFactory);
        }
    }
}