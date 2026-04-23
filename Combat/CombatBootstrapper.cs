using IdelPog.Combat.Assertion;
using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Contracts.Skill;
using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Resolver;
using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Factory;
using IdelPog.Combat.Mediator;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.Filter;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Factory;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Mediator;
using IdelPog.Combat.Runtime.System.Store;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Logging;
using IdelPog.Combat.Service.Logging.Interface;
using IdelPog.Core.Factory;
using IdelPog.Core.Flows.Registry;
using IdelPog.Core.Logging;
using IdelPog.Core.Logging.Writer;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Controller;
using IdelPog.Core.Messaging.Dispatcher;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat
{
    public static class CombatBootstrapper
    {
        public static void SetupCombat(IBufferManager bufferManager, IBatchRegister flowRegister)
        {
            IFoundAssertion foundAssertion = new FoundAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(foundAssertion, objectNullAssertion, uniqueAssertion);
            
            ILogWriter logWriter = new ConsoleWriter();
            IBufferLogger bufferLogger = new BufferLoggingService(logWriter);
            IAssetRepository<SkillType, SkillEntity> skillEntityRepository = new AssetRepository<SkillType, SkillEntity>(repositoryAsserter);
            
            SetupBasicEncounterDeck(bufferManager, flowRegister, bufferLogger, repositoryAsserter);
            SetupCombatantAbilities(bufferManager, flowRegister, bufferLogger, repositoryAsserter, skillEntityRepository);
        }

        private static void SetupBasicEncounterDeck(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, IRepositoryAsserter repositoryAsserter)
        {
            IFoundAssertion foundAssertion = new FoundAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            INumberAssertion numberAssertion = new NumberAssertion();
            
            ICombatantSelector lowHealthSelector = new LowestHealthSelector(collectionAssertion);
            ICombatantSelector highestAttackSelector = new HighestAttackSelector(collectionAssertion);
            CombatQueue combatQueue = new();
            
            CombatantRepository combatantRepository = new(foundAssertion);
            ICombatantStore friendlyCombatantStore = new CombatantStore(lowHealthSelector, highestAttackSelector, collectionAssertion, numberAssertion);
            ICombatantStore enemyCombatantStore = new CombatantStore(lowHealthSelector, highestAttackSelector, collectionAssertion, numberAssertion);
            
            ICombatantEntityFactory combatantEntityFactory = new CombatantEntityFactory(combatantRepository, collectionAssertion, uniqueAssertion, repositoryAsserter);
            ICombatantStoreService combatantStoreService = new CombatantStoreService(friendlyCombatantStore, enemyCombatantStore, combatantRepository, collectionAssertion);
            IBasicAttackScheduler basicAttackScheduler = new BasicAttackScheduler(combatQueue, numberAssertion, combatantRepository, foundAssertion);
            AssetRepository<EventType, IEventResolver> resolverRepository = new(repositoryAsserter);
            ICombatStateService combatStateService = new CombatStateService(combatantRepository);
            IDispatchMany<BasicEncounterDeckResponse> responseDispatcher = new ManagedDispatcher<BasicEncounterDeckResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            ICombatantLogger combatantLogger = new CombatantLogger(objectNullAssertion);
            
            // TODO: move this out eventually 
            EntityDamageMediator entityDamageMediator = CreateEntityDamageMediator(combatantRepository, friendlyCombatantStore, enemyCombatantStore, combatantStoreService, combatStateService, combatantLogger);
            BasicAttackEventResolver basicAttackEventResolver = new(entityDamageMediator, basicAttackScheduler, combatantRepository, foundAssertion);
            resolverRepository.Add(EventType.BASIC_ATTACK, basicAttackEventResolver);
            
            BasicEncounterDeckMediator basicEncounterDeckMediator = new(combatantEntityFactory, combatantStoreService, basicAttackScheduler, combatQueue, resolverRepository, combatStateService, collectionAssertion, responseDispatcher, combatantLogger);
            IBatchController<BasicEncounterDeck> controller = new ManagedBatchController<BasicEncounterDeck>(basicEncounterDeckMediator);
            BasicEncounterDeckErrorFactory errorFactory = new(new BaseErrorFactory());
                        
            flowRegister.RegisterBatch(controller, errorFactory);
        }

        private static EntityDamageMediator CreateEntityDamageMediator(CombatantRepository combatantRepository, ICombatantStore friendlyCombatantStore, ICombatantStore enemyCombatantStore, ICombatantStoreService combatantStoreService, ICombatStateService combatStateService, ICombatantLogger combatantLogger)
        {
            IFoundAssertion foundAssertion = new FoundAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICombatantAssertion combatantAssertion = new CombatantAssertion();
            INumberAssertion numberAssertion = new NumberAssertion();
            
            ITargetFinder targetFinder = new EnemyTargetFinder(friendlyCombatantStore, enemyCombatantStore, combatantRepository, objectNullAssertion, foundAssertion);
            IDamageSystem damageSystem = new DamageSystem();
            IDeathSystem deathSystem = new DeathSystem(combatStateService, combatantStoreService, combatantAssertion);
            
            return new EntityDamageMediator(combatantRepository, targetFinder, damageSystem, deathSystem, combatantStoreService, foundAssertion, combatantAssertion, numberAssertion, combatantLogger);
        }

        private static void SetupCombatantAbilities(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, IRepositoryAsserter repositoryAsserter, IAssetRepository<SkillType,SkillEntity> skillEntityRepository)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            
            ISkillEntityFactory skillEntityFactory = new SkillEntityFactory(repositoryAsserter);
            IDispatchMany<CombatantSkillCreationResponse> responseDispatcher = new ManagedDispatcher<CombatantSkillCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            CombatantSkillCreationMediator mediator = new(skillEntityRepository, skillEntityFactory, responseDispatcher, collectionAssertion, uniqueAssertion);
            IBatchController<CombatantSkillCreation> controller = new ManagedBatchController<CombatantSkillCreation>(mediator);
            CombatantSkillCreationErrorFactory errorFactory = new(new BaseErrorFactory());
            
            flowRegister.RegisterBatch(controller, errorFactory);
        }
    }
}