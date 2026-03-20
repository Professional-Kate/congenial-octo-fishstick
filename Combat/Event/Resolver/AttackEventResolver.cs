using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Event.Resolver
{
    public sealed class AttackEventResolver : IEventResolver
    {
        private readonly IDamageSystem _damageSystem;
        private readonly IAttackScheduler _attackScheduler;

        public AttackEventResolver(IDamageSystem damageSystem, IAttackScheduler attackScheduler)
        {
            _damageSystem = damageSystem;
            _attackScheduler = attackScheduler;
        }

        public EventType EventType => EventType.BASIC_ATTACK;
        
        public void ResolveEvent(double tick, byte attackerID)
        { 
            _damageSystem.ApplyDamage(attackerID);
            
            // TODO: only Enqueue if attacker is still alive (new method needed)
            _attackScheduler.EnqueueAttack(tick, attackerID);
        }
    }
}