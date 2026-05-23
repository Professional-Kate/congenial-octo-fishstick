using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Repository.Asset;

namespace IdelPog.Combat.Service
{
    public sealed class CombatQueueRunner : ICombatQueueRunner
    {
        public required uint MaxIterations { get; init; } 
        
        private readonly ICombatStateService _combatStateService;
        private readonly ICombatQueue _combatQueue;
        private readonly IAssetRepository<EventType, IEventResolver> _resolverRepository;

        public CombatQueueRunner(ICombatStateService combatStateService, ICombatQueue combatQueue, IAssetRepository<EventType, IEventResolver> resolverRepository)
        {
            _combatStateService = combatStateService;
            _combatQueue = combatQueue;
            _resolverRepository = resolverRepository;
        }

        public void RunDeck(BasicEncounterDeck basicEncounterDeck)
        {
            uint iterations = 0;
            while (_combatStateService.IsCombatOver == false)
            {
                if (++iterations > MaxIterations)
                {
                    throw new MaxIterationsException(basicEncounterDeck, MaxIterations);
                }
                    
                CombatEvent combatEvent = _combatQueue.Dequeue();
                double currentTick = combatEvent.Tick;

                IEventResolver resolver = _resolverRepository.Get(combatEvent.EventType);
                resolver.ResolveEvent(currentTick, combatEvent.AttackerID, combatEvent.AbilityType);
            }
        }
    }
}