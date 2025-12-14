using IdelPog.Combat.Assertion;
using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Factory;
using IdelPog.Combat.Factory.Interface;
using IdelPog.Combat.Mediator;
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
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat
{
    public static class CombatBootstrapper
    {
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="flowRegistry">Used to register the CurrencyCreation and CurrencyUpdate flows</param>
        public static void RegisterFlows(IBufferManager bufferManager, IBatchRegister flowRegistry)
        {
            IFoundAssertion foundAssertion = new FoundAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(foundAssertion, objectNullAssertion, uniqueAssertion);
            
            ILogWriter logWriter = new ConsoleWriter();
            IBufferLogger bufferLogger = new BufferLoggingService(logWriter);
            
            IStateRepository<ArenaType, Arena> arenaRepository = new StateRepository<ArenaType, Arena>(repositoryAsserter);
            IAssetRepository<CombatantType, CombatantDefinition> combatantRepository = new AssetRepository<CombatantType, CombatantDefinition>(repositoryAsserter);
            IAssetRepository<AbilityType, AbilityDefinition> abilityRepository = new AssetRepository<AbilityType, AbilityDefinition>(repositoryAsserter);
                
            RegisterArenaCreation(bufferManager, flowRegistry, bufferLogger, arenaRepository);
            RegisterCombatantDefinitionCreation(bufferManager, flowRegistry, bufferLogger, combatantRepository);
            RegisterAbilityDefinitionCreation(bufferManager, flowRegistry, bufferLogger, abilityRepository);
        }

        private static void RegisterArenaCreation(IBufferManager bufferManager, IBatchRegister flowRegistry, IBufferLogger bufferLogger, IStateRepository<ArenaType, Arena> arenaRepository)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            ILevelAssertion levelAssertion = new LevelAssertion();

            IArenaFactory arenaFactory = new ArenaFactory();
            IDispatchMany<ArenaCreationResponse> responseDispatcher = new ManagedDispatcher<ArenaCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            ArenaCreationMediator arenaCreationMediator = new(arenaRepository, arenaFactory, responseDispatcher, collectionAssertion, uniqueAssertion, levelAssertion);
            IBatchController<ArenaCreation> arenaCreationController = new ManagedBatchController<ArenaCreation>(arenaCreationMediator);
            
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            ArenaCreationErrorFactory errorFactory = new(baseErrorFactory);
            
            flowRegistry.RegisterBatch(arenaCreationController,  errorFactory);
        }

        private static void RegisterCombatantDefinitionCreation(IBufferManager bufferManager, IBatchRegister flowRegistry, IBufferLogger bufferLogger, IAssetRepository<CombatantType, CombatantDefinition> combatantRepository)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            ICombatantStatsAssertion combatantStatsAssertion = new CombatantStatsAssertion(new AmountAssertion());
            
            ICombatantDefinitionFactory combatantDefinitionFactory = new CombatantDefinitionFactory();
            IDispatchMany<CombatantDefinitionCreationResponse> responseDispatcher = new ManagedDispatcher<CombatantDefinitionCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            CombatantDefinitionCreationMediator combatantDefinitionCreationMediator = new(combatantRepository, combatantDefinitionFactory, responseDispatcher, collectionAssertion, uniqueAssertion, combatantStatsAssertion);
            IBatchController<CombatantDefinitionCreation> combatantDefinitionCreationController = new ManagedBatchController<CombatantDefinitionCreation>(combatantDefinitionCreationMediator);
            
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            CombatantDefinitionCreationErrorFactory combatantDefinitionCreationErrorFactory = new(baseErrorFactory);
            
            flowRegistry.RegisterBatch(combatantDefinitionCreationController, combatantDefinitionCreationErrorFactory);
        }

        private static void RegisterAbilityDefinitionCreation(IBufferManager bufferManager, IBatchRegister flowRegistry, IBufferLogger bufferLogger, IAssetRepository<AbilityType, AbilityDefinition> abilityRepository)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            IAmountAssertion amountAssertion = new AmountAssertion();
            
            IAbilityDefinitionFactory abilityDefinitionFactory = new AbilityDefinitionFactory();
            IDispatchMany<AbilityDefinitionCreationResponse> responseDispatcher = new ManagedDispatcher<AbilityDefinitionCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
                
            AbilityDefinitionCreationMediator abilityDefinitionCreationMediator = new(abilityRepository, abilityDefinitionFactory, responseDispatcher, collectionAssertion, uniqueAssertion, amountAssertion);
            IBatchController<AbilityDefinitionCreation> abilityDefinitionCreationController = new ManagedBatchController<AbilityDefinitionCreation>(abilityDefinitionCreationMediator);
            
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            AbilityDefinitionCreationErrorFactory abilityDefinitionCreationErrorFactory = new(baseErrorFactory);
            
            flowRegistry.RegisterBatch(abilityDefinitionCreationController, abilityDefinitionCreationErrorFactory);
        }
    }
}