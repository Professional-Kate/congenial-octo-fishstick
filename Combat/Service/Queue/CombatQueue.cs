using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Service.Queue.Interface;

namespace IdelPog.Combat.Service.Queue
{
    public sealed class CombatQueue : ICombatQueue, ICombatQueueClear
    {
        private readonly PriorityQueue<ScheduledCombatEvent, double> _combatQueue = new();
        
        public void Enqueue(ScheduledCombatEvent scheduledCombatEvent) => _combatQueue.Enqueue(scheduledCombatEvent, scheduledCombatEvent.Tick);
        
        public ScheduledCombatEvent Dequeue() => _combatQueue.Dequeue();
        
        public void Clear() => _combatQueue.Clear();
    }
}