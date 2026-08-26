using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.Event.Resolver.Interface;
using IdelPog.Combat.Runtime.Event.Trigger.Contracts;
using IdelPog.Combat.Runtime.Event.Trigger.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Repository.Asset;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class AbilityEventHandler : IAbilityEventHandler
    {
        private readonly ICombatantAbilityEntityRepository _combatantAbilityEntityRepository;
        private readonly ITriggerAbilityHandler<CombatantCastCompleteData> _combatantCastingHandler;
        private readonly IAbilityEventScheduler _abilityEventScheduler;
        private readonly IAssetRepository<AbilityEffectType, IAbilityEffectResolver> _resolverRepository;
        private readonly ICombatStateService _combatStateService;

        public AbilityEventHandler(ICombatantAbilityEntityRepository combatantAbilityEntityRepository,
            ITriggerAbilityHandler<CombatantCastCompleteData> combatantCastingHandler, IAbilityEventScheduler abilityEventScheduler,
            IAssetRepository<AbilityEffectType, IAbilityEffectResolver> resolverRepository, ICombatStateService combatStateService)
        {
            _combatantAbilityEntityRepository = combatantAbilityEntityRepository;
            _combatantCastingHandler = combatantCastingHandler;
            _abilityEventScheduler = abilityEventScheduler;
            _resolverRepository = resolverRepository;
            _combatStateService = combatStateService;
        }

        public void Handle(ScheduledCombatEvent scheduledCombatEvent)
        {
            CombatantAbilityEntity combatantAbilityEntity = _combatantAbilityEntityRepository.Get(scheduledCombatEvent.CombatantID, scheduledCombatEvent.AbilityID);
            if (scheduledCombatEvent.CombatEventType == CombatEventType.ABILITY_CAST_COMPLETE)
            { 
                _combatantCastingHandler.Handle(scheduledCombatEvent.Tick, new CombatantCastCompleteData { CastingCombatantID = scheduledCombatEvent.CombatantID, CombatantTargetingType = scheduledCombatEvent.TargetingType});
                _abilityEventScheduler.EnqueueAbilityExecuteEvent(scheduledCombatEvent.Tick, combatantAbilityEntity.AbilityID, scheduledCombatEvent.AbilityStageIndex, combatantAbilityEntity.CombatantID);
                return;
            }

            AbilityStagesComponent abilityStagesComponent = combatantAbilityEntity.GetComponent<AbilityStagesComponent>();
            CombatantAbilityStage currentStage = abilityStagesComponent.AbilityStages[scheduledCombatEvent.AbilityStageIndex];
                
            IAbilityEffectResolver resolver = _resolverRepository.Get(currentStage.AbilityStage.AbilityEffectType);
            resolver.ResolveEffect(scheduledCombatEvent.Tick, combatantAbilityEntity, currentStage);

            bool isLastStage = scheduledCombatEvent.AbilityStageIndex == abilityStagesComponent.AbilityStages.Length - 1;
            if (isLastStage)
            {
                ScheduleNextActivation(scheduledCombatEvent.Tick, combatantAbilityEntity);
                return;
            }

            byte nextStageIndex = (byte) (scheduledCombatEvent.AbilityStageIndex + 1);
            _abilityEventScheduler.ScheduleEvent(scheduledCombatEvent.Tick, combatantAbilityEntity.AbilityID, nextStageIndex, combatantAbilityEntity.CombatantID);
        }

        private void ScheduleNextActivation(double tick, CombatantAbilityEntity combatantAbilityEntity)
        {
            if (_combatStateService.IsCombatOver)
            {
                return;
            }
            
            TriggerComponent triggerComponent = combatantAbilityEntity.GetComponent<TriggerComponent>();
            if (triggerComponent.TriggerEventType != TriggerEventType.ABILITY_READY)
            {
                return;
            }

            CooldownComponent cooldownComponent = combatantAbilityEntity.GetComponent<CooldownComponent>();
            _abilityEventScheduler.ScheduleEvent(tick + cooldownComponent.Cooldown, combatantAbilityEntity.AbilityID, abilityStageIndex: 0, initiatingCombatantID: combatantAbilityEntity.CombatantID);
        }
    }
}