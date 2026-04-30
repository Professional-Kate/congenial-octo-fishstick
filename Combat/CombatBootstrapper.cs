using IdelPog.Combat.Assertion;
using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Resolver;
using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Factory;
using IdelPog.Combat.Factory.Interface;
using IdelPog.Combat.Mediator;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.Filter;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Factory;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Mediator;
using IdelPog.Combat.Runtime.System.Repository;
using IdelPog.Combat.Runtime.System.Repository.Interface;
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
        public static void RegisterFlows(IBufferManager bufferManager, IBatchRegister flowRegister, CombatOptions combatOptions)
        {
            IFoundAssertion foundAssertion = new FoundAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(foundAssertion, objectNullAssertion, uniqueAssertion);
            
            ILogWriter logWriter = new ConsoleWriter();
            IBufferLogger bufferLogger = new BufferLoggingService(logWriter);
            
            CombatantRepository combatantRepository = new(foundAssertion);
            IAssetRepository<AbilityType, AbilityEntity> abilityEntityRepository = new AssetRepository<AbilityType, AbilityEntity>(repositoryAsserter);
            ICombatantAbilityEntityRepository combatantAbilityEntityRepository = new CombatantAbilityEntityRepository(collectionAssertion, foundAssertion);
            
            RegisterBasicEncounterDeck(bufferManager, flowRegister, bufferLogger, repositoryAsserter, combatantRepository, combatantAbilityEntityRepository, combatOptions.MaxIterations);
            RegisterCombatantCreation(bufferManager, flowRegister, bufferLogger, combatantRepository);
            RegisterAbilityCreation(bufferManager, flowRegister, bufferLogger, repositoryAsserter, abilityEntityRepository);
            RegisterCombatantAbilityEquip(bufferManager, flowRegister, bufferLogger, combatantAbilityEntityRepository, abilityEntityRepository, combatOptions);
        }

        private static void RegisterBasicEncounterDeck(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, IRepositoryAsserter repositoryAsserter, CombatantRepository combatantRepository, ICombatantAbilityEntityRepository combatantAbilityEntityRepository, uint maxIterations)
        {
            IFoundAssertion foundAssertion = new FoundAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            INumberAssertion numberAssertion = new NumberAssertion();
            
            ICombatantSelector lowHealthSelector = new LowestHealthSelector(collectionAssertion);
            ICombatantSelector highestAttackSelector = new HighestAttackSelector(collectionAssertion);
            CombatQueue combatQueue = new();
            
            ICombatantStore friendlyCombatantStore = new CombatantStore(lowHealthSelector, highestAttackSelector, collectionAssertion, numberAssertion);
            ICombatantStore enemyCombatantStore = new CombatantStore(lowHealthSelector, highestAttackSelector, collectionAssertion, numberAssertion);

            IFriendlyStatusAssigner friendlyStatusAssigner = new FriendlyStatusAssigner(combatantRepository, collectionAssertion, foundAssertion);
            ICombatantStoreService combatantStoreService = new CombatantStoreService(friendlyCombatantStore, enemyCombatantStore, combatantRepository, collectionAssertion);
            IBasicAttackScheduler basicAttackScheduler = new AbilityScheduler(combatantRepository, combatantAbilityEntityRepository, combatQueue, numberAssertion, foundAssertion);
            AssetRepository<EventType, IEventResolver> resolverRepository = new(repositoryAsserter);
            ICombatStateService combatStateService = new CombatStateService(combatantRepository);
            IDispatchMany<BasicEncounterDeckResponse> responseDispatcher = new ManagedDispatcher<BasicEncounterDeckResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            ICombatantLogger combatantLogger = new CombatantLogger(objectNullAssertion);
            ICombatQueueRunner combatQueueRunner = new CombatQueueRunner(combatStateService, combatQueue, resolverRepository) { MaxIterations = maxIterations };
            EntityDamageMediator entityDamageMediator = CreateEntityDamageMediator(combatantRepository, friendlyCombatantStore, enemyCombatantStore, combatantStoreService, combatStateService, combatantLogger, combatantAbilityEntityRepository);
            
            // TODO: move this out eventually 
            DirectDamageEventResolver directDamageEventResolver = new(entityDamageMediator, basicAttackScheduler, combatantRepository, foundAssertion);
            resolverRepository.Add(EventType.BASIC_ATTACK, directDamageEventResolver);
            
            BasicEncounterDeckMediator basicEncounterDeckMediator = new(friendlyStatusAssigner, combatantStoreService, basicAttackScheduler, combatQueueRunner, combatStateService, combatantLogger, responseDispatcher, collectionAssertion);
            IBatchController<BasicEncounterDeck> controller = new ManagedBatchController<BasicEncounterDeck>(basicEncounterDeckMediator);
            BasicEncounterDeckErrorFactory errorFactory = new(new BaseErrorFactory());
                        
            flowRegister.RegisterBatch(controller, errorFactory);
        }

        private static EntityDamageMediator CreateEntityDamageMediator(CombatantRepository combatantRepository, ICombatantStore friendlyCombatantStore, ICombatantStore enemyCombatantStore, ICombatantStoreService combatantStoreService, ICombatStateService combatStateService, ICombatantLogger combatantLogger, ICombatantAbilityEntityRepository combatantAbilityEntityRepository)
        {
            IFoundAssertion foundAssertion = new FoundAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICombatantAssertion combatantAssertion = new CombatantAssertion();
            INumberAssertion numberAssertion = new NumberAssertion();
            
            ITargetFinder targetFinder = new EnemyTargetFinder(friendlyCombatantStore, enemyCombatantStore, combatantAbilityEntityRepository, combatantRepository, objectNullAssertion, foundAssertion);
            IDamageSystem damageSystem = new DamageSystem();
            IDeathSystem deathSystem = new DeathSystem(combatStateService, combatantStoreService, combatantAssertion);
            
            return new EntityDamageMediator(combatantRepository, targetFinder, damageSystem, combatantAbilityEntityRepository, deathSystem, combatantStoreService, combatantLogger, foundAssertion, combatantAssertion, numberAssertion);
        }

        private static void RegisterCombatantCreation(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, ICombatantRepository combatantRepository)
        {
            IFoundAssertion foundAssertion = new FoundAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            INumberAssertion numberAssertion = new NumberAssertion();
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(foundAssertion, objectNullAssertion, uniqueAssertion);
            IStatCardAsserter statCardAsserter = new StatCardAsserter(numberAssertion);
            
            ICombatantEntityFactory combatantEntityFactory = new CombatantEntityFactory(repositoryAsserter);
            IDispatchMany<CombatantCreationResponse> responseDispatcher =  new ManagedDispatcher<CombatantCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            CombatantCreationMediator mediator = new(combatantRepository, combatantEntityFactory, responseDispatcher, collectionAssertion, statCardAsserter);
            IBatchController<CombatantCreation> controller = new ManagedBatchController<CombatantCreation>(mediator);
            CombatantCreationErrorFactory errorFactory = new(new BaseErrorFactory());
            
            flowRegister.RegisterBatch(controller, errorFactory);
        }
        
        private static void RegisterAbilityCreation(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, IRepositoryAsserter repositoryAsserter, IAssetRepository<AbilityType, AbilityEntity> skillEntityRepository)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            INumberAssertion numberAssertion = new NumberAssertion();
            
            IAbilityEntityFactory abilityEntityFactory = new AbilityEntityFactory(repositoryAsserter);
            IDispatchMany<AbilityCreationResponse> responseDispatcher = new ManagedDispatcher<AbilityCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            AbilityCreationMediator mediator = new(skillEntityRepository, abilityEntityFactory, responseDispatcher, collectionAssertion, uniqueAssertion, numberAssertion);
            IBatchController<AbilityCreation> controller = new ManagedBatchController<AbilityCreation>(mediator);
            AbilityCreationErrorFactory errorFactory = new(new BaseErrorFactory());
            
            flowRegister.RegisterBatch(controller, errorFactory);
        }

        private static void RegisterCombatantAbilityEquip(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, ICombatantAbilityEntityRepository combatantAbilityEntityRepository, IAssetRepository<AbilityType,AbilityEntity> skillEntityRepository, CombatOptions combatOptions)
        {
            IFoundAssertion foundAssertion = new FoundAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            ICombatantAbilityAssertion combatantAbilityAssertion = new CombatantAbilityAssertion() { MaxAbilities = combatOptions.MaxCombatantAbilities };
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(foundAssertion, objectNullAssertion, uniqueAssertion);
            
            ICombatantAbilityEntityFactory combatantAbilityEntityFactory = new CombatantAbilityEntityFactory(skillEntityRepository, repositoryAsserter, foundAssertion);
            ICombatantAbilityFactory combatantAbilityFactory = new CombatantAbilityFactory();
            IDispatchMany<CombatantAbilityEquipResponse> responseDispatcher = new ManagedDispatcher<CombatantAbilityEquipResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            CombatantAbilityEquipMediator mediator = new(combatantAbilityEntityRepository, combatantAbilityEntityFactory, combatantAbilityFactory, responseDispatcher, collectionAssertion, combatantAbilityAssertion);
            IBatchController<CombatantAbilityEquip> controller = new ManagedBatchController<CombatantAbilityEquip>(mediator);
            CombatantAbilityEquipErrorFactory errorFactory = new(new BaseErrorFactory());
            
            flowRegister.RegisterBatch(controller, errorFactory);
        }
    }
}