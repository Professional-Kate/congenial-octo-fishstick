using IdelPog.Combat.Event;

namespace IdelPog.Combat.Service.Interface
{
    public interface IEnqueueEvent
    { 
        public void Enqueue(ICombatEvent combatEvent, double time);
    }
}