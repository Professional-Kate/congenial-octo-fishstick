using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Logging.Contracts;
using IdelPog.Combat.Core.Logging.Interface;
using IdelPog.Combat.Stat.Contracts.Enum;

namespace IdelPog.Combat.Core.Logging
{
    public sealed class ReadOnlyAbilityFactory : IReadOnlyAbilityFactory
    {
        public ReadOnlyAbility[] Create(AbilityEntity[] abilityEntities)
        {
            ReadOnlyAbility[] readOnlyAbilities = new ReadOnlyAbility[abilityEntities.Length];
            for (int i = 0; i < abilityEntities.Length; i++)
            {
                AbilityEntity abilityEntity = abilityEntities[i];
                readOnlyAbilities[i] = Create(abilityEntity);
            }

            return readOnlyAbilities;
        }

        private static ReadOnlyAbility Create(AbilityEntity abilityEntity)
        {
            AbilityCard abilityCard = new() { AbilitySlots = abilityEntity.GetStat(StatType.ABILITY_SLOTS), Cooldown = abilityEntity.GetStat(StatType.COOLDOWN) };
            
            ReadOnlyAbility readOnlyAbility = new()
            {
                AbilityCard = abilityCard,
                AbilityID = abilityEntity.AbilityID,
                InstanceID = abilityEntity.InstanceID,
                ReadOnlyAbilityStages = CreateAbilityStages(abilityEntity.GetComponent<AbilityStagesComponent>())
            };

            return readOnlyAbility;
        }

        private static ReadOnlyAbilityStage[] CreateAbilityStages(AbilityStagesComponent abilityStagesComponent)
        {
            ReadOnlyAbilityStage[] readOnlyAbilityStages = new ReadOnlyAbilityStage[abilityStagesComponent.AbilityStages.Length];
            for (int i = 0; i < abilityStagesComponent.AbilityStages.Length; i++)
            {
                AbilityStage abilityStage = abilityStagesComponent.AbilityStages[i];

                readOnlyAbilityStages[i] = new ReadOnlyAbilityStage
                {
                    AbilityEffectType = abilityStage.AbilityStageCard.AbilityEffectType,
                    AffinityType = abilityStage.AbilityStageCard.AffinityType,
                    ReadOnlyStrategy = CreateStrategy(abilityStage.TargetingPreferenceComponent),
                    CastTime = abilityStage.AbilityStageCard.CastTime,
                    Value = abilityStage.AbilityStageCard.Value,
                    MaxTargets = abilityStage.AbilityStageCard.MaxTargets
                };
            }
            
            return  readOnlyAbilityStages;
        }

        private static ReadOnlyStrategy CreateStrategy(TargetingPreferenceComponent targetingPreferenceComponent)
        {
            return new ReadOnlyStrategy
            {
                TargetingPreference = targetingPreferenceComponent.TargetingPreference,
                TargetingType = targetingPreferenceComponent.TargetingType,
                StatType = targetingPreferenceComponent.StatType
            };
        }
    }
}