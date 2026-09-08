using IdelPog.Combat.Ability.Contracts;
using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Stat.Runtime.Component;
using IdelPog.Core.Repository.Incremental;

namespace IdelPog.Combat.Ability.Runtime.System
{
    public sealed class AbilityEntityFactory : IAbilityEntityFactory
    {
        private readonly IIncrementalRepository<AbilityDefinition> _abilityDefinitionRepository;
        private readonly IAbilityStatsFactory _abilityStatsFactory;

        public AbilityEntityFactory(IIncrementalRepository<AbilityDefinition> abilityDefinitionRepository, IAbilityStatsFactory abilityStatsFactory)
        {
            _abilityDefinitionRepository = abilityDefinitionRepository;
            _abilityStatsFactory = abilityStatsFactory;
        }

        public AbilityEntity[] Create(EquippedAbilityDefinition equippedAbilityDefinition, byte instanceID)
        {
            List<AbilityEntity> combatantAbilityEntities = [];
            foreach (EquippedAbility equippedAbility in equippedAbilityDefinition.EquippedAbilities)
            {
                AbilityDefinition abilityDefinition = _abilityDefinitionRepository.Get(equippedAbility.AbilityID);

                AbilityStagesComponent abilityStagesComponent = new() { AbilityStages = [..ConvertAbilityStages(equippedAbility.StrategyCards, abilityDefinition)] };
                StatComponent[] statComponents = _abilityStatsFactory.CreateStats(abilityStagesComponent, abilityDefinition.AbilityCard);
                AbilityEntity abilityEntity = AddBaseComponents(abilityDefinition, instanceID, equippedAbility.AbilityID, abilityStagesComponent, statComponents);
                
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
                StatType = strategyCard.StatType, 
                TargetingPreference = strategyCard.TargetingPreference, 
                TargetingType = strategyCard.TargetingType
            };
                
            return new AbilityStage { AbilityStageCard = abilityStage, TargetingPreferenceComponent = targetingPreferenceComponent};
        }
        
        private static AbilityEntity AddBaseComponents(AbilityDefinition abilityDefinition, byte instanceID, byte abilityID, AbilityStagesComponent abilityStagesComponent, StatComponent[] statComponents)
        {
            TriggerCard triggerCard = abilityDefinition.TriggerCard;
            TriggerComponent triggerComponent = new()
            {
                TargetingType = triggerCard.TargetingType, 
                TriggerEventType = triggerCard.TriggerEventType, 
                MinTriggerValue =  triggerCard.MinTriggerValue, 
                MaxTriggerValue = triggerCard.MaxTriggerValue
            };

            StatsComponent statsComponent = new() { StatComponents =  statComponents };
            AbilityEntity abilityEntity = new(statsComponent, triggerComponent, abilityStagesComponent)
            {
                InstanceID = instanceID,
                AbilityID = abilityID
            };

            return abilityEntity;
        }
    }
}