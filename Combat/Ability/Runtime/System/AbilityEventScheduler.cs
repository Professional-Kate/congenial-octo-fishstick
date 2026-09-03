using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Ability.Service.Interface;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Service.Interface;

namespace IdelPog.Combat.Ability.Runtime.System
{
    public sealed class AbilityEventScheduler : IAbilityEventScheduler
    {
        private readonly IAbilityEntityRepository _abilityEntityRepository;
        private readonly IReadyTickSystem _readyTickSystem;
        private readonly ICombatantRepository _combatantRepository;
        private readonly ICastingCalculator _castingCalculator;
        private readonly ICombatQueue _combatQueue;

        public AbilityEventScheduler(IAbilityEntityRepository abilityEntityRepository, IReadyTickSystem readyTickSystem, ICombatantRepository combatantRepository, ICastingCalculator castingCalculator, ICombatQueue combatQueue)
        {
            _abilityEntityRepository = abilityEntityRepository;
            _readyTickSystem = readyTickSystem;
            _combatantRepository = combatantRepository;
            _castingCalculator = castingCalculator;
            _combatQueue = combatQueue;
        }

        public void ScheduleEvent(double forTick, byte abilityID, byte abilityStageIndex, byte initiatingCombatantID)
        {
            AbilityEntity abilityEntity = _abilityEntityRepository.Get(initiatingCombatantID, abilityID);
            CombatantEntity combatantEntity = _combatantRepository.Get(initiatingCombatantID);
            AgilityComponent agilityComponent = combatantEntity.GetComponent<AgilityComponent>();
            
            if (abilityStageIndex == 0)
            { 
                _readyTickSystem.SetNextReadyTick(forTick, abilityEntity, agilityComponent.Speed);
            }
           
            AbilityStage indexedStage = abilityEntity.GetComponent<AbilityStagesComponent>().AbilityStages[abilityStageIndex];
            if (indexedStage.AbilityStageCards.CastTime != 0)
            {
                EnqueueCastingEvent(abilityID, abilityStageIndex, initiatingCombatantID, combatantEntity.TargetingType, forTick, indexedStage.AbilityStageCards.CastTime, agilityComponent.Speed);
                return;
            }
            
            _combatQueue.Enqueue(CreateCombatEvent(CombatEventType.ABILITY_EXECUTE, abilityID, abilityStageIndex, initiatingCombatantID, combatantEntity.TargetingType, forTick));
        }
        
        public void EnqueueAbilityExecuteEvent(double forTick, byte abilityID, byte abilityStageIndex, byte initiatingCombatantID)
        {
            CombatantEntity combatantEntity = _combatantRepository.Get(initiatingCombatantID);
            ScheduledCombatEvent scheduledCombatEvent = CreateCombatEvent(CombatEventType.ABILITY_EXECUTE, abilityID, abilityStageIndex, initiatingCombatantID, combatantEntity.TargetingType, forTick);
            
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
                InstanceID = initiatingCombatantID, 
                TargetingType = targetingType,
                Tick = forTick
            };
    } 
}