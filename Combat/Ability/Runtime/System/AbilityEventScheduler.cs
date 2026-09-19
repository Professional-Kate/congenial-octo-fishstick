using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Ability.Service.Interface;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Stat.Contracts.Enum;

namespace IdelPog.Combat.Ability.Runtime.System
{
    public sealed class AbilityEventScheduler : IAbilityEventScheduler
    {
        private readonly IReadyTickSystem _readyTickSystem;
        private readonly ICastingCalculator _castingCalculator;
        private readonly ICombatQueue _combatQueue;

        public AbilityEventScheduler(IReadyTickSystem readyTickSystem, ICastingCalculator castingCalculator, ICombatQueue combatQueue)
        {
            _readyTickSystem = readyTickSystem;
            _castingCalculator = castingCalculator;
            _combatQueue = combatQueue;
        }

        public void ScheduleEvent(double forTick, AbilityEntity abilityEntity, byte abilityStageIndex, CombatantEntity combatantEntity)
        {
            uint combatantSpeed = combatantEntity.GetStat(StatType.SPEED);
            if (abilityStageIndex == 0)
            { 
                _readyTickSystem.SetNextReadyTick(forTick, abilityEntity, combatantSpeed);
            }
           
            AbilityStage indexedStage = abilityEntity.GetComponent<AbilityStagesComponent>().AbilityStages[abilityStageIndex];
            if (indexedStage.AbilityStageCard.CastTime != 0)
            {
                EnqueueCastingEvent(abilityEntity, abilityStageIndex, combatantEntity, forTick, indexedStage.AbilityStageCard.CastTime, combatantSpeed);
                return;
            }
            
            _combatQueue.Enqueue(CreateCombatEvent(CombatEventType.ABILITY_EXECUTE, abilityEntity, abilityStageIndex, combatantEntity, forTick));
        }
        
        public void EnqueueAbilityExecuteEvent(double forTick, AbilityEntity abilityEntity, byte abilityStageIndex, CombatantEntity combatantEntity)
        {
            ScheduledCombatEvent scheduledCombatEvent = CreateCombatEvent(CombatEventType.ABILITY_EXECUTE, abilityEntity, abilityStageIndex, combatantEntity, forTick);
            
            _combatQueue.Enqueue(scheduledCombatEvent);
        }

        private void EnqueueCastingEvent(AbilityEntity abilityEntity, byte abilityStageIndex, CombatantEntity combatantEntity, double forTick, uint castTime, uint combatantSpeed)
        {
            double castDuration = _castingCalculator.GetCastDuration(combatantSpeed, castTime);
            
            _combatQueue.Enqueue(CreateCombatEvent(CombatEventType.ABILITY_CAST_COMPLETE, abilityEntity, abilityStageIndex, combatantEntity, forTick + castDuration));
        }

        private static ScheduledCombatEvent CreateCombatEvent(CombatEventType combatEventType, AbilityEntity abilityEntity, byte abilityStageIndex, CombatantEntity combatantEntity, double forTick)
        {
            return new ScheduledCombatEvent
            {
                CombatEventType = combatEventType,
                AbilityEntity = abilityEntity,
                AbilityStageIndex = abilityStageIndex,
                CombatantEntity = combatantEntity,
                Tick = forTick
            };
        }
    } 
}