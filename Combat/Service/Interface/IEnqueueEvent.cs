using IdelPog.Combat.Event.Interface;

namespace IdelPog.Combat.Service.Interface
{
    public interface IEnqueueEvent
    { 
        public void Enqueue(ICombatEvent combatEvent, double tick);
    }
}