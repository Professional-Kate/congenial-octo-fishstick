using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entity;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Repository.Incremental;

namespace IdelPog.Combat.Runtime.System.Factory
{
    public sealed class AbilityEntityFactory : IAbilityEntityFactory
    {
        private readonly IIncrementalRepository<AbilityDefinition> _abilityDefinitionRepository;
        private readonly IAbilityEffectValueCalculator _abilityEffectValueCalculator;

        public AbilityEntityFactory(IIncrementalRepository<AbilityDefinition> abilityDefinitionRepository, IAbilityEffectValueCalculator abilityEffectValueCalculator)
        {
            _abilityDefinitionRepository = abilityDefinitionRepository;
            _abilityEffectValueCalculator = abilityEffectValueCalculator;
        }

        public AbilityEntity[] Create(EquippedAbilityDefinition equippedAbilityDefinition, byte instanceID)
        {
            List<AbilityEntity> combatantAbilityEntities = [];
            foreach (EquippedAbility equippedAbility in equippedAbilityDefinition.EquippedAbilities)
            {
                AbilityDefinition abilityDefinition = _abilityDefinitionRepository.Get(equippedAbility.AbilityID);

                AbilityStagesComponent abilityStagesComponent = new() { AbilityStages = [..ConvertAbilityStages(equippedAbility.StrategyCards, abilityDefinition)] };
                AbilityEntity abilityEntity = AddBaseComponents(abilityDefinition, instanceID, equippedAbility.AbilityID, abilityStagesComponent);
                _abilityEffectValueCalculator.Calculate(abilityEntity);
                
                combatantAbilityEntities.Add(abilityEntity);
            }
            
            return combatantAbilityEntities.ToArray();
        }

        private static AbilityStage[] ConvertAbilityStages(StrategyCard[] strategyCards, AbilityDefinition abilityDefinition)
        {
            AbilityStage[] combatantAbilityStages = new AbilityStage[abilityDefinition.AbilityStages.Length];
            for (int index = 0; index < abilityDefinition.AbilityStages.Length; index++)
            {
                combatantAbilityStages[index] = CreateCombatantAbilityStage(abilityDefinition.AbilityStages[index], strategyCards[index]);
            }
            
            return combatantAbilityStages;
        }

        private static AbilityStage CreateCombatantAbilityStage(AbilityStageCard abilityStage, StrategyCard strategyCard)
        {
            TargetingPreferenceComponent targetingPreferenceComponent = new()
            {
                CombatantStatType = strategyCard.CombatantStatType, 
                TargetingPreference = strategyCard.TargetingPreference, 
                TargetingType = strategyCard.TargetingType
            };
                
            return new AbilityStage { AbilityStageCards = abilityStage, TargetingPreferenceComponent = targetingPreferenceComponent};
        }
        
        private static AbilityEntity AddBaseComponents(AbilityDefinition abilityDefinition, byte instanceID, byte abilityID, AbilityStagesComponent abilityStagesComponent)
        {
            CooldownComponent cooldownComponent = new() { Cooldown = abilityDefinition.AbilityCard.Cooldown };
            
            TriggerCard triggerCard = abilityDefinition.TriggerCard;
            TriggerComponent triggerComponent = new()
            {
                TargetingType = triggerCard.TargetingType, 
                TriggerEventType = triggerCard.TriggerEventType, 
                MinTriggerValue =  triggerCard.MinTriggerValue, 
                MaxTriggerValue = triggerCard.MaxTriggerValue
            };
            
            AbilityEntity abilityEntity = new(cooldownComponent, triggerComponent, abilityStagesComponent)
            {
                InstanceID = instanceID,
                AbilityID = abilityID,
                AbilitySlots = abilityDefinition.AbilityCard.AbilitySlots
            };

            return abilityEntity;
        }
    }
}