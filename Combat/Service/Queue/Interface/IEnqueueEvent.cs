using IdelPog.Combat.Event;

namespace IdelPog.Combat.Service.Queue.Interface
{
    public interface IEnqueueEvent
    { 
        public void Enqueue(CombatEvent combatEvent);
    }
}