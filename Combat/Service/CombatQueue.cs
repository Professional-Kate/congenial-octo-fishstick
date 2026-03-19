using IdelPog.Combat.Event.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class CombatQueue : ICombatQueue
    {
        private readonly PriorityQueue<ICombatEvent, double> _combatQueue = new();
        
        public void Enqueue(ICombatEvent combatEvent, double tick) => _combatQueue.Enqueue(combatEvent, tick);
    }
}