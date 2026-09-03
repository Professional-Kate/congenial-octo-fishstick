using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Exceptions;

namespace IdelPog.Combat.Core.Service
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