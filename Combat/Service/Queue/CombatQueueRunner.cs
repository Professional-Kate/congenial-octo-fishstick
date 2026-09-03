using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Queue.Interface;

namespace IdelPog.Combat.Service.Queue
{
    public sealed class CombatQueueRunner : ICombatQueueRunner
    {
        public required uint MaxIterations { get; init; } 
        
        private readonly ICombatStateService _combatStateService;
        private readonly ICombatQueue _combatQueue;
        private readonly IAbilityEventHandler _abilityEventHandler;

        public CombatQueueRunner(ICombatStateService combatStateService, ICombatQueue combatQueue, IAbilityEventHandler abilityEventHandler)
        {
            _combatStateService = combatStateService;
            _combatQueue = combatQueue;
            _abilityEventHandler = abilityEventHandler;
        }

        public void RunCombat()
        {
            uint iterations = 0;
            while (_combatStateService.IsCombatOver == false)
            {
                if (++iterations > MaxIterations)
                {
                    throw new MaxIterationsException(MaxIterations);
                }
                    
                ScheduledCombatEvent scheduledCombatEvent = _combatQueue.Dequeue();
                _abilityEventHandler.Handle(scheduledCombatEvent);
            }
        }
    }
}