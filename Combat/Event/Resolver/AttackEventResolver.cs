using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Event.Resolver
{
    public sealed class AttackEventResolver : IEventResolver
    {
        private readonly IEntityDamageSystem _entityDamageSystem;
        private readonly IAttackScheduler _attackScheduler;
        private readonly ICombatantRepository _combatantRepository;
        private readonly IFoundAssertion _foundAssertion;

        public AttackEventResolver(IEntityDamageSystem entityDamageSystem, IAttackScheduler attackScheduler, ICombatantRepository combatantRepository, IFoundAssertion foundAssertion)
        {
            _entityDamageSystem = entityDamageSystem;
            _attackScheduler = attackScheduler;
            _combatantRepository = combatantRepository;
            _foundAssertion = foundAssertion;
        }

        public EventType EventType => EventType.BASIC_ATTACK;
        
        public void ResolveEvent(double tick, byte attackerID)
        { 
            _foundAssertion.AssertFound(attackerID, _combatantRepository.Contains(attackerID));
            
            if (_combatantRepository.Get(attackerID).GetComponent<LifeStatusComponent>().IsAlive == false)
            {
                return;
            }           
            
            _entityDamageSystem.ApplyDamage(attackerID);
            _attackScheduler.EnqueueAttack(tick, attackerID);
        }
    }
}