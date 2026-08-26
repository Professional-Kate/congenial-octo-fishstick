using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Queue.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class AbilityEventScheduler : IAbilityEventScheduler
    {
        private readonly ICombatantAbilityEntityRepository _combatantAbilityEntityRepository;
        private readonly IReadyTickSystem _readyTickSystem;
        private readonly ICombatantRepository _combatantRepository;
        private readonly ICastingCalculator _castingCalculator;
        private readonly ICombatQueue _combatQueue;

        public AbilityEventScheduler(ICombatantAbilityEntityRepository combatantAbilityEntityRepository, IReadyTickSystem readyTickSystem, ICombatantRepository combatantRepository, ICastingCalculator castingCalculator, ICombatQueue combatQueue)
        {
            _combatantAbilityEntityRepository = combatantAbilityEntityRepository;
            _readyTickSystem = readyTickSystem;
            _combatantRepository = combatantRepository;
            _castingCalculator = castingCalculator;
            _combatQueue = combatQueue;
        }

        public void ScheduleEvent(double forTick, byte abilityID, byte abilityStageIndex, byte initiatingCombatantID)
        {
            CombatantAbilityEntity combatantAbilityEntity = _combatantAbilityEntityRepository.Get(initiatingCombatantID, abilityID);
            CombatantEntity combatantEntity = _combatantRepository.Get(initiatingCombatantID);
            AgilityComponent agilityComponent = combatantEntity.GetComponent<AgilityComponent>();
            
            if (abilityStageIndex == 0)
            { 
                _readyTickSystem.SetNextReadyTick(forTick, combatantAbilityEntity, agilityComponent.Speed);
            }
           
            TargetingTypeComponent targetingTypeComponent = combatantEntity.GetComponent<TargetingTypeComponent>();
            
            CombatantAbilityStage indexedStage = combatantAbilityEntity.GetComponent<AbilityStagesComponent>().AbilityStages[abilityStageIndex];
            if (indexedStage.AbilityStage.CastTime != 0)
            {
                EnqueueCastingEvent(abilityID, abilityStageIndex, initiatingCombatantID, targetingTypeComponent.TargetingType, forTick, indexedStage.AbilityStage.CastTime, agilityComponent.Speed);
                return;
            }
            
            _combatQueue.Enqueue(CreateCombatEvent(CombatEventType.ABILITY_EXECUTE, abilityID, abilityStageIndex, initiatingCombatantID, targetingTypeComponent.TargetingType, forTick));
        }
        
        public void EnqueueAbilityExecuteEvent(double forTick, byte abilityID, byte abilityStageIndex, byte initiatingCombatantID)
        {
            CombatantEntity combatantEntity = _combatantRepository.Get(initiatingCombatantID);
            TargetingTypeComponent targetingTypeComponent = combatantEntity.GetComponent<TargetingTypeComponent>();
            ScheduledCombatEvent scheduledCombatEvent = CreateCombatEvent(CombatEventType.ABILITY_EXECUTE, abilityID, abilityStageIndex, initiatingCombatantID, targetingTypeComponent.TargetingType, forTick);
            
            _combatQueue.Enqueue(scheduledCombatEvent);
        }

        private void EnqueueCastingEvent(byte abilityID, byte abilityStageIndex, byte combatantID, TargetingType targetingType, double forTick, uint castTime, uint combatantSpeed)
        {
            double castDuration = _castingCalculator.GetCastDuration(combatantSpeed, castTime);
            
            _combatQueue.Enqueue(CreateCombatEvent(CombatEventType.ABILITY_CAST_COMPLETE, abilityID, abilityStageIndex, combatantID, targetingType, forTick + castDuration));
        }
        
        private static ScheduledCombatEvent CreateCombatEvent(CombatEventType combatEventType, byte abilityID, byte abilityStageIndex, byte initiatingCombatantID, TargetingType targetingType, double forTick) 
            => new()
            {
                CombatEventType = combatEventType, 
                AbilityID = abilityID, 
                AbilityStageIndex = abilityStageIndex, 
                CombatantID = initiatingCombatantID, 
                TargetingType = targetingType,
                Tick = forTick
            };
    } 
}