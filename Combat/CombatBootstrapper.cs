using IdelPog.Combat.Assertion;
using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Repository.Asserter;
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
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(foundAssertion, objectNullAssertion, uniqueAssertion);

            CombatantRepository combatantRepository = new(foundAssertion);
            ITargetFinder targetFinder = new TargetFinder(combatantRepository, new Random());
            CombatQueue combatQueue = new();
            IDamageSystem damageSystem = new DamageSystem(combatantRepository, foundAssertion, numberAssertion, targetFinder);
            
            ICombatantFactory combatantFactory = new CombatantFactory(combatantRepository, collectionAssertion, uniqueAssertion, repositoryAsserter);
            IAttackScheduler attackScheduler = new AttackScheduler(combatQueue, numberAssertion, combatantRepository, foundAssertion);
            
            CombatService combatService = new(collectionAssertion, combatantFactory, attackScheduler, combatQueue);

            return combatService;
        }
    }
}