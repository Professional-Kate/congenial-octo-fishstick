using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Event.Resolver
{
    public sealed class BasicAttackEventResolver : IEventResolver
    {
        private readonly IEntityDamageMediator _entityDamageMediator;
        private readonly IBasicAttackScheduler _basicAttackScheduler;
        private readonly ICombatantRepository _combatantRepository;
        private readonly IFoundAssertion _foundAssertion;

        public BasicAttackEventResolver(IEntityDamageMediator entityDamageMediator, IBasicAttackScheduler basicAttackScheduler, ICombatantRepository combatantRepository, IFoundAssertion foundAssertion)
        {
            _entityDamageMediator = entityDamageMediator;
            _basicAttackScheduler = basicAttackScheduler;
            _combatantRepository = combatantRepository;
            _foundAssertion = foundAssertion;
        }

        public void ResolveEvent(double tick, byte attackerID)
        { 
            _foundAssertion.AssertFound(attackerID, _combatantRepository.Contains(attackerID));
            
            if (_combatantRepository.Get(attackerID).GetComponent<LifeStatusComponent>().IsAlive == false)
            {
                return;
            }           
            
            _entityDamageMediator.ApplyDamage(attackerID, AbilityType.BASIC_ATTACK);
            _basicAttackScheduler.EnqueueAttack(tick, attackerID);
        }
    }
}