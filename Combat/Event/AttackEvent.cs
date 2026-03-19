using IdelPog.Combat.Event.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Event
{
    public sealed class AttackEvent : ICombatEvent
    {
        private readonly IDamageSystem _damageSystem;

        public readonly byte CombatantID;
        public double Tick { get; }
        
        public AttackEvent(IDamageSystem damageSystem, double tick, byte combatantID)
        {
            CombatantID = combatantID;
            _damageSystem = damageSystem;

            Tick = tick;
        }

        public void RunEvent(IEnqueueEvent enqueueEvent)
        { 
            _damageSystem.ApplyDamage(CombatantID);
        }
    }
}