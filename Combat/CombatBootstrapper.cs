using IdelPog.Combat.Ability.Contracts.Command;
using IdelPog.Combat.Ability.Contracts.Response;
using IdelPog.Combat.Ability.Mediator;
using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Assertion;
using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts.Response;
using IdelPog.Combat.Combatant.Mediator;
using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Core.Arena;
using IdelPog.Combat.Factory;
using IdelPog.Combat.Mediator;
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
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(foundAssertion, objectNullAssertion, uniqueAssertion);
            
            ILogWriter logWriter = new ConsoleWriter();
            IBufferLogger bufferLogger = new BufferLoggingService(logWriter);
            
            CombatantRepository combatantRepository = new();
            IIncrementalRepository<AbilityDefinition> abilityDefinitionRepository =  new IncrementalRepository<AbilityDefinition>(repositoryAsserter);
            IIncrementalRepository<CombatantDefinition> combatantDefinitionRepository = new IncrementalRepository<CombatantDefinition>(repositoryAsserter);
            Dictionary<byte, EquippedAbilityDefinition> equippedAbilityDefinitionRepository = new();
            
            IAbilityEntityRepository abilityEntityRepository = new AbilityEntityRepository();
            IAssetRepository<CombatantStatType, IStatProvider> statProviderRepository = new AssetRepository<CombatantStatType, IStatProvider>(repositoryAsserter);
            IPrioritySorter prioritySorter = new PrioritySorter();
            
            // TODO: move this out eventually 
            statProviderRepository.Add(CombatantStatType.HEALTH, new HealthProvider());
            statProviderRepository.Add(CombatantStatType.SPEED, new SpeedProvider());
            statProviderRepository.Add(CombatantStatType.INITIATIVE, new InitiativeProvider());
            statProviderRepository.Add(CombatantStatType.ABILITY_DAMAGE, new AbilityDamageProvider(abilityEntityRepository));
            statProviderRepository.Add(CombatantStatType.ABILITY_HEALING, new AbilityHealingProvider(abilityEntityRepository));
            
            RegisterBasicEncounterDeck(bufferManager, flowRegister, bufferLogger, repositoryAsserter, combatantRepository, abilityEntityRepository, statProviderRepository, combatOptions.MaxIterations, combatantDefinitionRepository, equippedAbilityDefinitionRepository, abilityDefinitionRepository, prioritySorter);
            RegisterCombatantCreation(bufferManager, flowRegister, bufferLogger, combatantDefinitionRepository);
            RegisterAbilityCreation(bufferManager, flowRegister,  bufferLogger, abilityDefinitionRepository, prioritySorter);
            RegisterAbilityEquip(bufferManager, flowRegister, bufferLogger, abilityDefinitionRepository, combatOptions, equippedAbilityDefinitionRepository);
        }

        private static void RegisterBasicEncounterDeck(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, IRepositoryAsserter repositoryAsserter, CombatantRepository combatantRepository, IAbilityEntityRepository abilityEntityRepository, IAssetRepository<CombatantStatType, IStatProvider> statProviderRepository, uint maxIterations,  IIncrementalRepository<CombatantDefinition> combatantDefinitionRepository, Dictionary<byte, EquippedAbilityDefinition> equippedAbilityDefinitionRepository, IIncrementalRepository<AbilityDefinition> abilityDefinitionRepository, IPrioritySorter prioritySorter)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            INumberAssertion numberAssertion = new NumberAssertion();
            ICombatantAssertion combatantAssertion = new CombatantAssertion();

            CombatQueue combatQueue = new();
            TriggerSubscriber triggerSubscriber = new();
            ICastingCalculator castingCalculator = new CastingCalculator();
            IReadyTickSystem readyTickSystem = new ReadyTickSystem(castingCalculator);
            IAbilityEventScheduler abilityEventScheduler = new AbilityEventScheduler(abilityEntityRepository, readyTickSystem, combatantRepository, castingCalculator, combatQueue);
            ITriggerAbilityHandler<CombatantCastCompleteData> combatantCastingHandler = new CombatantCastingHandler(triggerSubscriber, abilityEventScheduler, combatantRepository);
            IAssetRepository<AbilityEffectType, IAbilityEffectResolver> resolverRepository = new AssetRepository<AbilityEffectType, IAbilityEffectResolver>(repositoryAsserter);
            ICombatStateService combatStateService = new CombatStateService(combatantRepository);
            IAbilityEventHandler abilityEventHandler = new AbilityEventHandler(abilityEntityRepository, combatantCastingHandler, abilityEventScheduler, resolverRepository, combatStateService);
            IAbilityInitializer abilityInitializer = new AbilityInitializer();
            IInitialAbilityScheduler initialAbilityScheduler = new InitialAbilityScheduler(combatantRepository, abilityEntityRepository, abilityInitializer, abilityEventScheduler, triggerSubscriber);
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
            ICombatantEntityFactory combatantEntityFactory = new CombatantEntityFactory();
            IAbilityEffectValueCalculator abilityEffectValueCalculator = new AbilityEffectValueCalculator();
            IAbilityEntityFactory abilityEntityFactory = new AbilityEntityFactory(abilityDefinitionRepository, abilityEffectValueCalculator);
            
            // TODO: move this out eventually 
            DirectDamageAbilityEffectResolver directDamageAbilityEffectResolver = new(combatantRepository, targetFinder, combatantLogger, entityDamageService);
            resolverRepository.Add(AbilityEffectType.DIRECT_DAMAGE, directDamageAbilityEffectResolver);

            HealingAbilityEffectResolver healingAbilityEffectResolver = new(combatantRepository, targetFinder, combatantLogger, entityHealingService);
            resolverRepository.Add(AbilityEffectType.HEALING, healingAbilityEffectResolver);

            RetaliationAbilityEffectResolver retaliationAbilityEffectResolver = new(combatantRepository, targetFinder, combatantLogger, entityDamageService);
            resolverRepository.Add(AbilityEffectType.RETALIATION, retaliationAbilityEffectResolver);

            CombatArena combatArena = new(combatantEntityFactory, combatantRepository, equippedAbilityDefinitionRepository, abilityEntityFactory, abilityEntityRepository, initialAbilityScheduler, combatQueueRunner);
            
            BasicEncounterDeckMediator basicEncounterDeckMediator = new(combatantDefinitionRepository, combatArena, combatStateService, combatantLogger, responseDispatcher, collectionAssertion);
            IBatchController<BasicEncounterDeck> controller = new ManagedBatchController<BasicEncounterDeck>(basicEncounterDeckMediator);
            BasicEncounterDeckErrorFactory errorFactory = new(new BaseErrorFactory());
                        
            flowRegister.RegisterBatch(controller, errorFactory);
        }

        private static void RegisterCombatantCreation(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, IIncrementalRepository<CombatantDefinition> combatantDefinitionRepository)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            INumberAssertion numberAssertion = new NumberAssertion();
            ICardAsserter cardAsserter = new CardAsserter(numberAssertion);
            
            IDispatchMany<CombatantCreationResponse> responseDispatcher =  new ManagedDispatcher<CombatantCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            CombatantCreationMediator mediator = new(combatantDefinitionRepository, responseDispatcher, collectionAssertion, cardAsserter);
            IBatchController<CombatantCreation> controller = new ManagedBatchController<CombatantCreation>(mediator);
            CombatantCreationErrorFactory errorFactory = new(new BaseErrorFactory());
            
            flowRegister.RegisterBatch(controller, errorFactory);
        }
        
        private static void RegisterAbilityCreation(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, IIncrementalRepository<AbilityDefinition> abilityDefinitionRepository, IPrioritySorter prioritySorter)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            INumberAssertion numberAssertion = new NumberAssertion();
            ITriggerAssertion triggerAssertion = new TriggerAssertion();
            
            IDispatchMany<AbilityCreationResponse> responseDispatcher = new ManagedDispatcher<AbilityCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            AbilityCreationMediator mediator = new(abilityDefinitionRepository, prioritySorter, responseDispatcher, collectionAssertion, numberAssertion, triggerAssertion);
            IBatchController<AbilityCreation> controller = new ManagedBatchController<AbilityCreation>(mediator);
            AbilityCreationErrorFactory errorFactory = new(new BaseErrorFactory());
            
            flowRegister.RegisterBatch(controller, errorFactory);
        }

        private static void RegisterAbilityEquip(IBufferManager bufferManager, IBatchRegister flowRegister, IBufferLogger bufferLogger, IIncrementalRepository<AbilityDefinition> abilityDefinitionRepository, CombatOptions combatOptions, Dictionary<byte, EquippedAbilityDefinition> equippedAbilityDefinitionRepository)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            IAbilityAssertion abilityAssertion = new AbilityAssertion { MaxAbilitiesSlots = combatOptions.MaxCombatantAbilitySlots };
            IAbilitySlotCalculator abilitySlotCalculator = new AbilitySlotCalculator(abilityDefinitionRepository);
            
            IDispatchMany<AbilityEquipResponse> responseDispatcher = new ManagedDispatcher<AbilityEquipResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            AbilityEquipMediator mediator = new(abilitySlotCalculator, new PrioritySorter(), abilityDefinitionRepository, equippedAbilityDefinitionRepository, responseDispatcher, collectionAssertion, abilityAssertion, new PriorityAssertion());
            IBatchController<AbilityEquip> controller = new ManagedBatchController<AbilityEquip>(mediator);
            AbilityEquipErrorFactory errorFactory = new(new BaseErrorFactory());
            
            flowRegister.RegisterBatch(controller, errorFactory);
        }
    }
}