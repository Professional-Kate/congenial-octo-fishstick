using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Service.Interface;

namespace IdelPog.Combat.Core.Service
{
    public sealed class CombatQueue : ICombatQueue
    {
        private readonly PriorityQueue<ScheduledCombatEvent, double> _combatQueue = new();
        
        public void Enqueue(ScheduledCombatEvent scheduledCombatEvent) => _combatQueue.Enqueue(scheduledCombatEvent, scheduledCombatEvent.Tick);
        
        public ScheduledCombatEvent Dequeue() => _combatQueue.Dequeue();
    }
}