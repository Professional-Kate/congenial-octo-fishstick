using IdelPog.Combat.Assertion;
using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Factory;
using IdelPog.Combat.Factory.Interface;
using IdelPog.Combat.Mediator;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.Event.Resolver;
using IdelPog.Combat.Runtime.Event.Resolver.Interface;
using IdelPog.Combat.Runtime.Event.Trigger;
using IdelPog.Combat.Runtime.Event.Trigger.Contracts;
using IdelPog.Combat.Runtime.Event.Trigger.Handler;
using IdelPog.Combat.Runtime.Event.Trigger.Interface;
using IdelPog.Combat.Runtime.Filter;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.Filter.Provider;
using IdelPog.Combat.Runtime.Filter.Provider.Interface;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Factory;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Runtime.System.Interface;
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
using IdelPog.Core.Repository.Incremental;
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
            IIncrementalRepository<AbilityEntity> abilityEntityRepository = new IncrementalRepository<AbilityEntity>(new Dictionary<byte, AbilityEntity>(), repositoryAsserter);
            ICombatantAbilityEntityRepository combatantAbilityEntityRepository = new CombatantAbilityEntityRepository(collectionAssertion, foundAssertion);
            IAssetRepository<CombatantStatType, IStatProvider> statProviderRepository = new AssetRepository<CombatantStatType, IStatProvider>(repositoryAsserter);
            IPrioritySorter prioritySorter = new PrioritySorter();
            
            // TODO: move this out eventually 
            statProviderRepository.Add(CombatantStatType.HEALTH, new HealthProvider());
            statProviderRepository.Add(CombatantStatType.SPEED, new SpeedProvider());
            statProviderRepository.Add(CombatantStatType.INITIATIVE, new InitiativeProvider());
            statProviderRepository.Add(CombatantStatType.ABILITY_DAMAGE, new AbilityDamageProvider(combatantAbilityEntityRepository));
            statProviderRepository.Add(CombatantStatType.ABILITY_HEALING, new AbilityHealingProvider(combatantAbilityEntityRepository));
            
            RegisterBasicEncounterDeck(bufferManager, flowRegister, bufferLogger, repositoryAsserter, combatantRepository, combatantAbilityEntityRepository, statProviderRepository, combatOptions.MaxIterations);
            RegisterCombatantCreation(bufferManager, flowRegister, bufferLogger, combatantRepository);
            RegisterAbilityCreation(bufferManager, flowRegister,  bufferLogger, abilityEntityRepository, prioritySorter);
            RegisterCombatantAbilityEquip(bufferManager, flowRegister, bufferLogger, combatantAbilityEntityRepository, abilityEntityRepository, prioritySorter, combatOptions);
        }

        private static void RegisterBasicEncounterDeck(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, IRepositoryAsserter repositoryAsserter, CombatantRepository combatantRepository, ICombatantAbilityEntityRepository combatantAbilityEntityRepository, IAssetRepository<CombatantStatType, IStatProvider> statProviderRepository, uint maxIterations)
        {
            IFoundAssertion foundAssertion = new FoundAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            INumberAssertion numberAssertion = new NumberAssertion();
            ICombatantAssertion combatantAssertion = new CombatantAssertion();
            
            CombatQueue combatQueue = new();
            IDictionary<TriggerEventType, IList<CombatantAbilityEntity>> subscribedAbilities = new Dictionary<TriggerEventType, IList<CombatantAbilityEntity>>();
            TriggerSubscriber triggerSubscriber = new(subscribedAbilities);
            ICastingCalculator castingCalculator = new CastingCalculator();
            IReadyTickSystem readyTickSystem = new ReadyTickSystem(castingCalculator);
            IAbilityEventScheduler abilityEventScheduler = new AbilityEventScheduler(combatantAbilityEntityRepository, readyTickSystem, combatantRepository, castingCalculator, combatQueue);
            ITriggerAbilityHandler<CombatantCastCompleteData> combatantCastingHandler = new CombatantCastingHandler(triggerSubscriber, abilityEventScheduler, combatantRepository);
;            IAssetRepository<AbilityEffectType, IAbilityEffectResolver> resolverRepository = new AssetRepository<AbilityEffectType, IAbilityEffectResolver>(repositoryAsserter);
            ICombatStateService combatStateService = new CombatStateService(combatantRepository);
            IAbilityEventHandler abilityEventHandler = new AbilityEventHandler(combatantAbilityEntityRepository, combatantCastingHandler, abilityEventScheduler, resolverRepository, combatStateService);
            ICombatantAbilityInitializer combatantAbilityInitializer = new CombatantAbilityInitializer();
            IFriendlyStatusAssigner friendlyStatusAssigner = new FriendlyStatusAssigner(combatantRepository, collectionAssertion, foundAssertion);
            IInitialAbilityScheduler initialAbilityScheduler = new InitialAbilityScheduler(combatantRepository, combatantAbilityEntityRepository, combatantAbilityInitializer, abilityEventScheduler, triggerSubscriber);
            IDispatchMany<BasicEncounterDeckResponse> responseDispatcher = new ManagedDispatcher<BasicEncounterDeckResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            ICombatantLogger combatantLogger = new CombatantLogger(objectNullAssertion, collectionAssertion);
            ICombatQueueRunner combatQueueRunner = new CombatQueueRunner(combatStateService, combatQueue, abilityEventHandler) { MaxIterations = maxIterations };
            IDamageSystem damageSystem = new DamageSystem();
            IDeathSystem deathSystem = new DeathSystem(combatStateService, combatantAssertion);
            ITriggerAbilityHandler<CombatantDamagedData> combatantDamagedHandler = new CombatantDamagedHandler(triggerSubscriber, abilityEventScheduler, combatantRepository);
            ITriggerAbilityHandler<CombatantDeathData> combatantDiedHandler = new CombatantDeathHandler(triggerSubscriber, abilityEventScheduler, combatantRepository);
            IEntityDamageService entityDamageService = new EntityDamageService(damageSystem, combatantDamagedHandler, deathSystem, combatantDiedHandler);
            IEntityHealingService entityHealingService = new EntityHealingService();
            ICombatantTargetFinder targetFinder = new CombatantTargetFinder(combatantRepository, statProviderRepository, numberAssertion, collectionAssertion);
            ITearDownService tearDownService = new TearDownService(combatantRepository, combatantAbilityEntityRepository, combatQueue);
            
            // TODO: move this out eventually 
            DirectDamageAbilityEffectResolver directDamageAbilityEffectResolver = new(combatantRepository, targetFinder, combatantLogger, entityDamageService);
            resolverRepository.Add(AbilityEffectType.DIRECT_DAMAGE, directDamageAbilityEffectResolver);

            HealingAbilityEffectResolver healingAbilityEffectResolver = new(combatantRepository, targetFinder, combatantLogger, entityHealingService);
            resolverRepository.Add(AbilityEffectType.HEALING, healingAbilityEffectResolver);

            RetaliationAbilityEffectResolver retaliationAbilityEffectResolver = new(combatantRepository, targetFinder, combatantLogger, entityDamageService);
            resolverRepository.Add(AbilityEffectType.RETALIATION, retaliationAbilityEffectResolver);
            
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
        
        private static void RegisterAbilityCreation(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, IIncrementalRepository<AbilityEntity> skillEntityRepository, IPrioritySorter prioritySorter)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            INumberAssertion numberAssertion = new NumberAssertion();
            ITriggerAssertion triggerAssertion = new TriggerAssertion();
            
            IAbilityEntityFactory abilityEntityFactory = new AbilityEntityFactory(prioritySorter);
            IDispatchMany<AbilityCreationResponse> responseDispatcher = new ManagedDispatcher<AbilityCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            AbilityCreationMediator mediator = new(skillEntityRepository, abilityEntityFactory, responseDispatcher, collectionAssertion, numberAssertion, triggerAssertion);
            IBatchController<AbilityCreation> controller = new ManagedBatchController<AbilityCreation>(mediator);
            AbilityCreationErrorFactory errorFactory = new(new BaseErrorFactory());
            
            flowRegister.RegisterBatch(controller, errorFactory);
        }

        private static void RegisterCombatantAbilityEquip(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, ICombatantAbilityEntityRepository combatantAbilityEntityRepository, IIncrementalRepository<AbilityEntity> abilityEntityRepository, IPrioritySorter prioritySorter, CombatOptions combatOptions)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            IPriorityAssertion priorityAssertion = new PriorityAssertion();
            ICombatantAbilityAssertion combatantAbilityAssertion = new CombatantAbilityAssertion { MaxAbilitiesSlots = combatOptions.MaxCombatantAbilitySlots };

            IAbilityEffectValueCalculator abilityEffectValueCalculator = new AbilityEffectValueCalculator();
            IAbilitySlotCalculator abilitySlotCalculator = new AbilitySlotCalculator(abilityEntityRepository);
            ICombatantAbilityEntityFactory combatantAbilityEntityFactory = new CombatantAbilityEntityFactory(abilityEntityRepository, prioritySorter, abilityEffectValueCalculator, priorityAssertion);
            ICombatantAbilityFactory combatantAbilityFactory = new CombatantAbilityFactory();
            IDispatchMany<CombatantAbilityEquipResponse> responseDispatcher = new ManagedDispatcher<CombatantAbilityEquipResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            CombatantAbilityEquipMediator mediator = new(abilitySlotCalculator, combatantAbilityEntityRepository, combatantAbilityEntityFactory, combatantAbilityFactory, responseDispatcher, collectionAssertion, combatantAbilityAssertion);
            IBatchController<CombatantAbilityEquip> controller = new ManagedBatchController<CombatantAbilityEquip>(mediator);
            CombatantAbilityEquipErrorFactory errorFactory = new(new BaseErrorFactory());
            
            flowRegister.RegisterBatch(controller, errorFactory);
        }
    }
}