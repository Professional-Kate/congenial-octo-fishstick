using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Event;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Queue.Interface;
using IdelPog.Core.Repository.Asset;

namespace IdelPog.Combat.Service
{
    public sealed class AbilityEventScheduler : IAbilityEventScheduler
    {
        private readonly ICombatantAbilityEntityRepository _combatantAbilityEntityRepository;
        private readonly ICombatantRepository _combatantRepository;
        private readonly IAssetRepository<AbilityType, EventType> _eventRepository;
        private readonly ICombatQueue _combatQueue;
        private readonly INumberAssertion _numberAssertion;

        public AbilityEventScheduler(ICombatantAbilityEntityRepository combatantAbilityEntityRepository, ICombatantRepository combatantRepository, IAssetRepository<AbilityType, EventType> eventRepository, ICombatQueue combatQueue, INumberAssertion numberAssertion)
        {
            _combatantAbilityEntityRepository = combatantAbilityEntityRepository;
            _combatantRepository = combatantRepository;
            _eventRepository = eventRepository;
            _combatQueue = combatQueue;
            _numberAssertion = numberAssertion;
        }

        public void ScheduleEvent(double currentTick, byte initiatingCombatantID, AbilityType abilityType)
        {
            CombatantAbilityEntity combatantAbilityEntity = _combatantAbilityEntityRepository.Get(initiatingCombatantID, abilityType);
            AgilityComponent agilityComponent = _combatantRepository.Get(initiatingCombatantID).GetComponent<AgilityComponent>();
            _numberAssertion.AssertNumberNotZero(agilityComponent.Speed, nameof(agilityComponent.Speed));

            if (combatantAbilityEntity.TryGetComponent(out CastTimeComponent castTimeComponent))
            {
                _numberAssertion.AssertNumberNotZero(castTimeComponent.CastTime, nameof(castTimeComponent.CastTime));
                
                EnqueueCastingEvent(currentTick,initiatingCombatantID, abilityType, castTimeComponent.CastTime, agilityComponent.Speed);
                return;
            }

            EnqueueAbilityEvent(currentTick, initiatingCombatantID, abilityType);
        }
        
        public void EnqueueAbilityEvent(double currentTick, byte initiatingCombatantID, AbilityType abilityType)
        {
            CombatEvent combatEvent = CreateCombatEvent(currentTick, initiatingCombatantID, abilityType, _eventRepository.Get(abilityType));
            
            _combatQueue.Enqueue(combatEvent);
        }

        private void EnqueueCastingEvent(double currentTick, byte attackerID, AbilityType abilityType, double castTime, uint combatantSpeed)
        {
            const double speedScalingFactor = 0.01;
            double nextTick = currentTick + castTime * speedScalingFactor / combatantSpeed;

            _combatQueue.Enqueue(CreateCombatEvent(nextTick, attackerID, abilityType, EventType.CASTING));
        }
        
        private static CombatEvent CreateCombatEvent(double forTick, byte initiatingCombatantID, AbilityType abilityType, EventType eventType) 
            => new() { AbilityType = abilityType, EventType = eventType, Tick = forTick, AttackerID = initiatingCombatantID };
    } 
}