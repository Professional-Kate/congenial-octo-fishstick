using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Event.Resolver.Interface;
using IdelPog.Combat.Core.Event.Trigger.Contracts;
using IdelPog.Combat.Core.Event.Trigger.Interface;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Core.Repository.Asset;

namespace IdelPog.Combat.Ability.Runtime.System
{
    public sealed class AbilityEventHandler : IAbilityEventHandler
    {
        private readonly IAbilityEntityRepository _abilityEntityRepository;
        private readonly ITriggerAbilityHandler<CombatantCastCompleteData> _combatantCastingHandler;
        private readonly IAbilityEventScheduler _abilityEventScheduler;
        private readonly IAssetRepository<AbilityEffectType, IAbilityEffectResolver> _resolverRepository;
        private readonly ICombatStateService _combatStateService;

        public AbilityEventHandler(IAbilityEntityRepository abilityEntityRepository,
            ITriggerAbilityHandler<CombatantCastCompleteData> combatantCastingHandler, IAbilityEventScheduler abilityEventScheduler,
            IAssetRepository<AbilityEffectType, IAbilityEffectResolver> resolverRepository, ICombatStateService combatStateService)
        {
            _abilityEntityRepository = abilityEntityRepository;
            _combatantCastingHandler = combatantCastingHandler;
            _abilityEventScheduler = abilityEventScheduler;
            _resolverRepository = resolverRepository;
            _combatStateService = combatStateService;
        }

        public void Handle(ScheduledCombatEvent scheduledCombatEvent)
        {
            AbilityEntity abilityEntity = _abilityEntityRepository.Get(scheduledCombatEvent.InstanceID, scheduledCombatEvent.AbilityID);
            if (scheduledCombatEvent.CombatEventType == CombatEventType.ABILITY_CAST_COMPLETE)
            { 
                _combatantCastingHandler.Handle(scheduledCombatEvent.Tick, new CombatantCastCompleteData { CastingCombatantID = scheduledCombatEvent.InstanceID, CombatantTargetingType = scheduledCombatEvent.TargetingType});
                _abilityEventScheduler.EnqueueAbilityExecuteEvent(scheduledCombatEvent.Tick, abilityEntity.AbilityID, scheduledCombatEvent.AbilityStageIndex, abilityEntity.InstanceID);
                return;
            }

            AbilityStagesComponent abilityStagesComponent = abilityEntity.GetComponent<AbilityStagesComponent>();
            AbilityStage currentStage = abilityStagesComponent.AbilityStages[scheduledCombatEvent.AbilityStageIndex];
                
            IAbilityEffectResolver resolver = _resolverRepository.Get(currentStage.AbilityStageCards.AbilityEffectType);
            resolver.ResolveEffect(scheduledCombatEvent.Tick, abilityEntity, currentStage);

            bool isLastStage = scheduledCombatEvent.AbilityStageIndex == abilityStagesComponent.AbilityStages.Length - 1;
            if (isLastStage)
            {
                ScheduleNextActivation(scheduledCombatEvent.Tick, abilityEntity);
                return;
            }

            byte nextStageIndex = (byte) (scheduledCombatEvent.AbilityStageIndex + 1);
            _abilityEventScheduler.ScheduleEvent(scheduledCombatEvent.Tick, abilityEntity.AbilityID, nextStageIndex, abilityEntity.InstanceID);
        }

        private void ScheduleNextActivation(double tick, AbilityEntity abilityEntity)
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

            CooldownComponent cooldownComponent = abilityEntity.GetComponent<CooldownComponent>();
            _abilityEventScheduler.ScheduleEvent(tick + cooldownComponent.Cooldown, abilityEntity.AbilityID, abilityStageIndex: 0, initiatingCombatantID: abilityEntity.InstanceID);
        }
    }
}