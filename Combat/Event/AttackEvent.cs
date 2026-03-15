using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Event
{
    public sealed class AttackEvent : ICombatEvent
    {
        private readonly CombatantCard _attacker;
        private readonly CombatantCard _target;

        public AttackEvent(CombatantCard attacker, CombatantCard target)
        {
            _attacker = attacker;
            _target = target;
        }

        public void RunEvent(IEnqueueEvent enqueueEvent, double tick)
        {
            // call into the ECS and reduce HP for the _target

            double interval = 1.0 / _attacker.StatCard.Speed;
            double nexTick = tick + interval;
            
            enqueueEvent.Enqueue(new AttackEvent(_attacker, _target), nexTick);
        }
    }
}