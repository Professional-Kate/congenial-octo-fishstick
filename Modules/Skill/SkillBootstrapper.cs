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
using IdelPog.Skill.Service.Interface;

namespace IdelPog.Skill
{
    public static class SkillBootstrapper
    {
        public static void RegisterFlows(IBufferManager bufferManager, FlowRegister flowRegistry)
        {
            CurrentSkillProvider skillProvider = new();
            
            ILogWriter writer = new ConsoleWriter();
            IBufferLogger bufferLogger = new BufferLoggingService(writer);
            
            IStateRepository<SkillID, Contracts.Skill> skillRepository = new StateRepository<SkillID, Contracts.Skill>();
            
            RegisterSetSkill(bufferManager, skillProvider, flowRegistry, bufferLogger, skillRepository);
            RegisterScheduleTick(bufferManager, skillProvider, flowRegistry, skillRepository, bufferLogger);
            RegisterSkillCreation(bufferManager, flowRegistry, skillRepository, bufferLogger);
        }
        
        /// <summary>
        /// Registers the <see cref="SetSkill"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="SetSkillResponse"/></param>
        /// <param name="currentSkillSetter">Used together with <see cref="ICurrentSkillProvider"/></param>
        /// <param name="flowRegistry">Used to register the SetSkill flow</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <param name="skillRepository">Used to store Skills</param>
        /// <remarks>
        /// Listens to -> <see cref="SetSkill"/>. On Success -> <see cref="SetSkillResponse"/>. On Error -> <see cref="SetSkillError"/>
        /// </remarks>
        private static void RegisterSetSkill(IBufferManager bufferManager, ICurrentSkillSetter  currentSkillSetter, ISingleRegister flowRegistry, IBufferLogger bufferLogger, IStateRepository<SkillID, Contracts.Skill> skillRepository)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<SetSkillError, SetSkill> setSkillErrorFactory = new SetSkillErrorFactory(baseErrorFactory );
            ILevelProgressFactory levelProgressFactory = new LevelProgressFactory();

            ISetSkillResponseFactory setSkillResponseFactory = new SetSkillResponseFactory(levelProgressFactory);
            IDispatchOne<SetSkillResponse> setSkillResponseDispatcher = new ManagedDispatcher<SetSkillResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            ISingleMediator<SetSkill> setSkillMediator = new SetSkillMediator(currentSkillSetter, skillRepository, setSkillResponseFactory, setSkillResponseDispatcher, foundAssertion);
            ISingleController<SetSkill> setSkillController = new ManagedSingleController<SetSkill>(setSkillMediator);
            
            flowRegistry.RegisterSingle(setSkillController, setSkillErrorFactory);
        }

        /// <summary>
        /// Registers the <see cref="SkillCreation"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="SetSkillResponse"/></param>
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
            ILevelableAssertionPipeline levelableAssertionPipeline = new LevelableAssertionPipeline(levelAssertion, objectNullAssertion);
            
            ISkillCreationResponseFactory responseFactory = new SkillCreationResponseFactory();
            IDispatchOne<SkillCreationResponse> responseDispatcher = new ManagedDispatcher<SkillCreationResponse>(bufferManager, bufferLogger,  objectNullAssertion, collectionAssertion);
                
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<SkillCreationError, IReadOnlyList<SkillCreation>> errorFactory = new SkillCreationErrorFactory(baseErrorFactory);
            
            IBatchMediator<SkillCreation> mediator = new SkillCreationMediator(skillRepository, responseFactory, responseDispatcher, levelableAssertionPipeline, collectionAssertion, uniqueAssertion);
            IBatchController<SkillCreation> controller = new ManagedBatchController<SkillCreation>(mediator);
            
            flowRegistry.RegisterBatch(controller, errorFactory);
        }

        /// <summary>
        /// Registers the <see cref="ScheduleTick"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="SetSkillResponse"/></param>
        /// <param name="currentSkillProvider">Used together with <see cref="ICurrentSkillSetter"/></param>
        /// <param name="flowRegistry">Used to register the ScheduleTick flow</param>
        /// <param name="skillRepository">Used to store Skills</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// /// <remarks>
        /// Listens to -> <see cref="ScheduleTick"/>. On Success -> <see cref="SkillUpdateResponse"/>. On Error -> <see cref="SkillUpdateError"/>.
        /// </remarks>
        private static void RegisterScheduleTick(IBufferManager bufferManager, ICurrentSkillProvider currentSkillProvider, ISingleRegister flowRegistry, IStateRepository<SkillID, Contracts.Skill> skillRepository, IBufferLogger bufferLogger)
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
            IDispatchOne<SkillUpdateResponse> responseDispatcher = new ManagedDispatcher<SkillUpdateResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
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
            IErrorFactory<SkillUpdateError, ScheduleTick> updateErrorFactory = new SkillUpdateErrorFactory(baseErrorFactory);
            ISingleMediator<ScheduleTick> skillActionMediator = new SkillActionMediator(experienceService, levelService, skillRepository, currentSkillProvider, responseDispatcher, updateResponseFactory, lootService);
            ISingleController<ScheduleTick> skillActionController = new ManagedSingleController<ScheduleTick>(skillActionMediator);
            
            flowRegistry.RegisterSingle(skillActionController, updateErrorFactory);
        }
    }
}