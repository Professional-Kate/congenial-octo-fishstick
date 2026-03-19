using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Event.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Event
{
    public sealed class AttackEvent : ICombatEvent
    {
        private readonly IDamageSystem _damageSystem;
        private readonly StatCard _attackerStats;
        public readonly byte TargetID;

        public double Tick { get; }
        
        public AttackEvent(IDamageSystem damageSystem, StatCard attackerStats, byte targetID, double tick)
        {
            _attackerStats = attackerStats;
            TargetID = targetID;
            _damageSystem = damageSystem;

            Tick = tick;
        }

        public void RunEvent(IEnqueueEvent enqueueEvent)
        {
            _damageSystem.ApplyDamage(TargetID, _attackerStats);
        }
    }
}