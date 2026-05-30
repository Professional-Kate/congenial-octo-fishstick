using IdelPog.Combat.Assertion;
using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
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
using IdelPog.Combat.Runtime.Filter.Provider;
using IdelPog.Combat.Runtime.Filter.Provider.Interface;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Factory;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Mediator;
using IdelPog.Combat.Runtime.System.Mediator.Interface;
using IdelPog.Combat.Runtime.System.Repository;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Logging;
using IdelPog.Combat.Service.Logging.Interface;
using IdelPog.Combat.Service.Queue;
using IdelPog.Combat.Service.Queue.Interface;
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
            IAssetRepository<AbilityType, EventType> eventRepository =  new AssetRepository<AbilityType, EventType>(repositoryAsserter);
            ICombatantAbilityEntityRepository combatantAbilityEntityRepository = new CombatantAbilityEntityRepository(collectionAssertion, foundAssertion);
            IAssetRepository<CombatantStatType, IStatProvider> statProviderRepository = new AssetRepository<CombatantStatType, IStatProvider>(repositoryAsserter);
            
            // TODO: move this out eventually 
            statProviderRepository.Add(CombatantStatType.HEALTH, new HealthProvider());
            statProviderRepository.Add(CombatantStatType.SPEED, new SpeedProvider());
            statProviderRepository.Add(CombatantStatType.INITIATIVE, new InitiativeProvider());
            
            RegisterBasicEncounterDeck(bufferManager, flowRegister, bufferLogger, repositoryAsserter, combatantRepository, combatantAbilityEntityRepository, eventRepository, statProviderRepository, combatOptions.MaxIterations);
            RegisterCombatantCreation(bufferManager, flowRegister, bufferLogger, combatantRepository);
            RegisterAbilityCreation(bufferManager, flowRegister,  bufferLogger, abilityEntityRepository, eventRepository);
            RegisterCombatantAbilityEquip(bufferManager, flowRegister, bufferLogger, combatantAbilityEntityRepository, abilityEntityRepository, combatOptions);
        }

        private static void RegisterBasicEncounterDeck(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, IRepositoryAsserter repositoryAsserter, CombatantRepository combatantRepository, ICombatantAbilityEntityRepository combatantAbilityEntityRepository, IAssetRepository<AbilityType, EventType> eventRepository, IAssetRepository<CombatantStatType, IStatProvider> statProviderRepository, uint maxIterations)
        {
            IFoundAssertion foundAssertion = new FoundAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            INumberAssertion numberAssertion = new NumberAssertion();
            ICombatantAssertion combatantAssertion = new CombatantAssertion();
            
            CombatQueue combatQueue = new();
            IFriendlyStatusAssigner friendlyStatusAssigner = new FriendlyStatusAssigner(combatantRepository, collectionAssertion, foundAssertion);
            IAbilityEventScheduler abilityEventScheduler = new AbilityEventScheduler(combatantAbilityEntityRepository, combatantRepository, eventRepository, combatQueue, numberAssertion);
            IInitialAbilityScheduler initialAbilityScheduler = new InitialAbilityScheduler(combatantRepository, combatantAbilityEntityRepository, abilityEventScheduler, numberAssertion);
            IAssetRepository<EventType, IEventResolver> resolverRepository = new AssetRepository<EventType, IEventResolver>(repositoryAsserter);
            ICombatStateService combatStateService = new CombatStateService(combatantRepository);
            IDispatchMany<BasicEncounterDeckResponse> responseDispatcher = new ManagedDispatcher<BasicEncounterDeckResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            ICombatantLogger combatantLogger = new CombatantLogger(objectNullAssertion);
            ICombatQueueRunner combatQueueRunner = new CombatQueueRunner(combatStateService, combatQueue, resolverRepository) { MaxIterations = maxIterations };
            IDamageSystem damageSystem = new DamageSystem();
            IDeathSystem deathSystem = new DeathSystem(combatStateService, combatantAssertion);
            IEntityDamageMediator entityDamageMediator = new EntityDamageMediator(damageSystem, deathSystem, combatantLogger);
            ICombatantTargetFinder targetFinder = new CombatantTargetFinder(combatantRepository, statProviderRepository, numberAssertion, collectionAssertion);
            ITearDownService tearDownService = new TearDownService(combatantRepository, combatQueue);
            
            // TODO: move this out eventually 
            DirectDamageEventResolver directDamageEventResolver = new(targetFinder, combatantAbilityEntityRepository, entityDamageMediator, combatantRepository, abilityEventScheduler);
            resolverRepository.Add(EventType.DIRECT_DAMAGE, directDamageEventResolver);

            CastingEventResolver castingEventResolver = new(abilityEventScheduler);
            resolverRepository.Add(EventType.CASTING, castingEventResolver);
            
            BasicEncounterDeckMediator basicEncounterDeckMediator = new(friendlyStatusAssigner, initialAbilityScheduler, combatQueueRunner, combatStateService, combatantLogger, responseDispatcher, collectionAssertion, tearDownService);
            IBatchController<BasicEncounterDeck> controller = new ManagedBatchController<BasicEncounterDeck>(basicEncounterDeckMediator);
            BasicEncounterDeckErrorFactory errorFactory = new(new BaseErrorFactory());
                        
            flowRegister.RegisterBatch(controller, errorFactory);
        }

        private static void RegisterCombatantCreation(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, ICombatantRepository combatantRepository)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            INumberAssertion numberAssertion = new NumberAssertion();
            ICardAsserter cardAsserter = new CardAsserter(numberAssertion);
            
            ICombatantEntityFactory combatantEntityFactory = new CombatantEntityFactory();
            IDispatchMany<CombatantCreationResponse> responseDispatcher =  new ManagedDispatcher<CombatantCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            CombatantCreationMediator mediator = new(combatantRepository, combatantEntityFactory, responseDispatcher, collectionAssertion, cardAsserter);
            IBatchController<CombatantCreation> controller = new ManagedBatchController<CombatantCreation>(mediator);
            CombatantCreationErrorFactory errorFactory = new(new BaseErrorFactory());
            
            flowRegister.RegisterBatch(controller, errorFactory);
        }
        
        private static void RegisterAbilityCreation(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, IAssetRepository<AbilityType, AbilityEntity> skillEntityRepository, IAssetRepository<AbilityType, EventType> eventRepository)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            INumberAssertion numberAssertion = new NumberAssertion();
            
            IAbilityEntityFactory abilityEntityFactory = new AbilityEntityFactory();
            IDispatchMany<AbilityCreationResponse> responseDispatcher = new ManagedDispatcher<AbilityCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            AbilityCreationMediator mediator = new(skillEntityRepository, abilityEntityFactory, eventRepository, responseDispatcher, collectionAssertion, uniqueAssertion, numberAssertion);
            IBatchController<AbilityCreation> controller = new ManagedBatchController<AbilityCreation>(mediator);
            AbilityCreationErrorFactory errorFactory = new(new BaseErrorFactory());
            
            flowRegister.RegisterBatch(controller, errorFactory);
        }

        private static void RegisterCombatantAbilityEquip(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, ICombatantAbilityEntityRepository combatantAbilityEntityRepository, IAssetRepository<AbilityType,AbilityEntity> abilityEntityRepository, CombatOptions combatOptions)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            ICombatantAbilityAssertion combatantAbilityAssertion = new CombatantAbilityAssertion() { MaxAbilitiesSlots = combatOptions.MaxCombatantAbilitySlots };

            IAbilitySlotCalculator abilitySlotCalculator = new AbilitySlotCalculator(abilityEntityRepository);
            ICombatantAbilityEntityFactory combatantAbilityEntityFactory = new CombatantAbilityEntityFactory(abilityEntityRepository);
            ICombatantAbilityFactory combatantAbilityFactory = new CombatantAbilityFactory();
            IDispatchMany<CombatantAbilityEquipResponse> responseDispatcher = new ManagedDispatcher<CombatantAbilityEquipResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            CombatantAbilityEquipMediator mediator = new(abilitySlotCalculator, combatantAbilityEntityRepository, combatantAbilityEntityFactory, combatantAbilityFactory, responseDispatcher, collectionAssertion, combatantAbilityAssertion);
            IBatchController<CombatantAbilityEquip> controller = new ManagedBatchController<CombatantAbilityEquip>(mediator);
            CombatantAbilityEquipErrorFactory errorFactory = new(new BaseErrorFactory());
            
            flowRegister.RegisterBatch(controller, errorFactory);
        }
    }
}