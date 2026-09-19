using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Event.Resolver.Interface;
using IdelPog.Combat.Core.Event.Trigger.Contracts;
using IdelPog.Combat.Core.Event.Trigger.Interface;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Core.Repository.Asset;

namespace IdelPog.Combat.Ability.Runtime.System
{
    public sealed class AbilityEventHandler : IAbilityEventHandler
    {
        private readonly ITriggerAbilityHandler<CombatantCastCompleteData> _combatantCastingHandler;
        private readonly IAbilityEventScheduler _abilityEventScheduler;
        private readonly IAssetRepository<AbilityEffectType, IAbilityEffectResolver> _resolverRepository;
        private readonly ICombatStateService _combatStateService;

        public AbilityEventHandler(ITriggerAbilityHandler<CombatantCastCompleteData> combatantCastingHandler, IAbilityEventScheduler abilityEventScheduler,
            IAssetRepository<AbilityEffectType, IAbilityEffectResolver> resolverRepository, ICombatStateService combatStateService)
        {
            _combatantCastingHandler = combatantCastingHandler;
            _abilityEventScheduler = abilityEventScheduler;
            _resolverRepository = resolverRepository;
            _combatStateService = combatStateService;
        }

        public void Handle(ScheduledCombatEvent scheduledCombatEvent)
        {
            AbilityEntity abilityEntity = scheduledCombatEvent.AbilityEntity;
            CombatantEntity combatantEntity = scheduledCombatEvent.CombatantEntity;
            
            if (scheduledCombatEvent.CombatEventType == CombatEventType.ABILITY_CAST_COMPLETE)
            { 
                _combatantCastingHandler.Handle(scheduledCombatEvent.Tick, new CombatantCastCompleteData { CombatantTargetingType = combatantEntity.TargetingType});
                _abilityEventScheduler.EnqueueAbilityExecuteEvent(scheduledCombatEvent.Tick, abilityEntity, scheduledCombatEvent.AbilityStageIndex, combatantEntity);
                return;
            }

            AbilityStagesComponent abilityStagesComponent = abilityEntity.GetComponent<AbilityStagesComponent>();
            AbilityStage currentStage = abilityStagesComponent.AbilityStages[scheduledCombatEvent.AbilityStageIndex];
                
            IAbilityEffectResolver resolver = _resolverRepository.Get(currentStage.AbilityStageCard.AbilityEffectType);
            resolver.ResolveEffect(scheduledCombatEvent.Tick, abilityEntity, currentStage, combatantEntity);

            bool isLastStage = scheduledCombatEvent.AbilityStageIndex == abilityStagesComponent.AbilityStages.Length - 1;
            if (isLastStage)
            {
                ScheduleNextActivation(scheduledCombatEvent.Tick, abilityEntity, combatantEntity);
                return;
            }

            byte nextStageIndex = (byte) (scheduledCombatEvent.AbilityStageIndex + 1);
            _abilityEventScheduler.ScheduleEvent(scheduledCombatEvent.Tick, abilityEntity, nextStageIndex, combatantEntity);
        }

        private void ScheduleNextActivation(double tick, AbilityEntity abilityEntity, CombatantEntity combatantEntity)
        {
            if (_combatStateService.IsCombatOver)
            {
                return;
            }
            
            TriggerComponent triggerComponent = abilityEntity.GetComponent<TriggerComponent>();
            if (triggerComponent.TriggerEventType != TriggerEventType.ABILITY_READY)
            {
                return;
            }

            _abilityEventScheduler.ScheduleEvent(tick + abilityEntity.GetStat(StatType.COOLDOWN), abilityEntity, abilityStageIndex: 0, combatantEntity);
        }
    }
}