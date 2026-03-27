using IdelPog.Combat.Assertion;
using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Resolver;
using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Runtime.Filter;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat
{
    public static class CombatBootstrapper
    {
        public static ICombatService SetupCombat()
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
            ICombatLog combatLog = new CombatLog();
            
            ICombatantFactory combatantFactory = new CombatantFactory(combatantRepository, collectionAssertion, uniqueAssertion, repositoryAsserter);
            IAttackScheduler attackScheduler = new AttackScheduler(combatQueue, numberAssertion, combatantRepository, foundAssertion);
            IAssetRepository<EventType, IEventResolver> resolverRepository = new AssetRepository<EventType, IEventResolver>(repositoryAsserter);
            ICombatantStoreService combatantStoreService = new CombatantStoreService(friendlyCombatantStore, enemyCombatantStore, combatantRepository, collectionAssertion);
            ICombatStateService combatStateService = new CombatStateService(combatantRepository);
            IDamageSystem damageSystem = new DamageSystem();
            IDeathSystem deathSystem = new DeathSystem(combatStateService, combatantStoreService, combatantAssertion);
            
            IEntityDamageMediator entityDamageMediator = new EntityDamageMediator(combatantRepository, targetFinder, damageSystem, combatLog, deathSystem, combatantStoreService, foundAssertion, combatantAssertion, numberAssertion);
            // TODO: move this out eventually 
            AttackEventResolver attackEventResolver = new(entityDamageMediator, attackScheduler, combatantRepository, foundAssertion);
            resolverRepository.Add(EventType.BASIC_ATTACK, attackEventResolver);
            
            CombatService combatService = new(combatantFactory, combatantStoreService, attackScheduler, combatQueue, resolverRepository, combatStateService, collectionAssertion, combatLog);

            return combatService;
        }
    }
}