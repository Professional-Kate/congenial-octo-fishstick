using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Event
{
    public sealed class AttackEvent : ICombatEvent
    {
        private readonly IDamageSystem _damageSystem;
        private readonly StatCard _attackerStats;
        private readonly byte _targetID;

        public AttackEvent(IDamageSystem damageSystem, StatCard attackerStats, byte targetID)
        {
            _attackerStats = attackerStats;
            _targetID = targetID;
            _damageSystem = damageSystem;
        }

        public void RunEvent(IEnqueueEvent enqueueEvent, double tick)
        {
            _damageSystem.ApplyDamage(_targetID, _attackerStats);
            // double interval = 1.0 / _attacker.StatCard.Speed;
            // double nexTick = tick + interval;
        }
    }
}