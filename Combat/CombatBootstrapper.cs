using IdelPog.Combat.Assertion;
using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Resolver;
using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Factory;
using IdelPog.Combat.Runtime.Filter;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System;
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
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            IFoundAssertion foundAssertion = new FoundAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            INumberAssertion numberAssertion = new NumberAssertion();
            ICombatantAssertion combatantAssertion = new CombatantAssertion();
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(foundAssertion, objectNullAssertion, uniqueAssertion);

            ICombatantSelector lowHealthSelector = new LowestHealthSelector(collectionAssertion);
            ICombatantSelector highestAttackSelector = new HighestAttackSelector(collectionAssertion);
            ICombatantStore friendlyCombatantStore = new CombatantStore(lowHealthSelector, highestAttackSelector, collectionAssertion, numberAssertion);
            ICombatantStore enemyCombatantStore = new CombatantStore(lowHealthSelector, highestAttackSelector, collectionAssertion, numberAssertion);
            CombatantRepository combatantRepository = new(foundAssertion);
            ITargetFinder targetFinder = new TargetFinder(friendlyCombatantStore, enemyCombatantStore, combatantRepository, objectNullAssertion);
            CombatQueue combatQueue = new();
            ISkillComponentFactory skillComponentFactory = new SkillComponentFactory();
            
            ICombatantFactory combatantFactory = new CombatantFactory(combatantRepository, skillComponentFactory, collectionAssertion, uniqueAssertion, repositoryAsserter);
            IBasicAttackScheduler basicAttackScheduler = new BasicAttackScheduler(combatQueue, numberAssertion, combatantRepository, foundAssertion);
            AssetRepository<EventType, IEventResolver> resolverRepository = new(repositoryAsserter);
            ICombatantStoreService combatantStoreService = new CombatantStoreService(friendlyCombatantStore, enemyCombatantStore, combatantRepository, collectionAssertion);
            ICombatStateService combatStateService = new CombatStateService(combatantRepository);
            IDamageSystem damageSystem = new DamageSystem();
            IDeathSystem deathSystem = new DeathSystem(combatStateService, combatantStoreService, combatantAssertion);

            IBufferLogger bufferLogger = new BufferLoggingService(new ConsoleWriter());
            
            ICombatantLogger combatantLogger = new CombatantLogger(objectNullAssertion);
            IDispatchMany<BasicEncounterDeckResponse> responseDispatcher = new ManagedDispatcher<BasicEncounterDeckResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            IEntityDamageMediator entityDamageMediator = new EntityDamageMediator(combatantRepository, targetFinder, damageSystem, deathSystem, combatantStoreService, foundAssertion, combatantAssertion, numberAssertion, combatantLogger);
            // TODO: move this out eventually 
            BasicAttackEventResolver basicAttackEventResolver = new(entityDamageMediator, basicAttackScheduler, combatantRepository, foundAssertion);
            resolverRepository.Add(EventType.BASIC_ATTACK, basicAttackEventResolver);
            
            BasicEncounterDeckMediator basicEncounterDeckMediator = new(combatantFactory, combatantStoreService, basicAttackScheduler, combatQueue, resolverRepository, combatStateService, collectionAssertion, responseDispatcher, combatantLogger);
            IBatchController<BasicEncounterDeck> controller = new ManagedBatchController<BasicEncounterDeck>(basicEncounterDeckMediator);
            BasicEncounterDeckErrorFactory errorFactory = new(new BaseErrorFactory());
                        
            flowRegister.RegisterBatch(controller, errorFactory);
        }
    }
}