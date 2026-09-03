using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Core.Event;

namespace IdelPog.Combat.Ability.Runtime.System
{
    public sealed class AbilityEffectValueCalculator : IAbilityEffectValueCalculator
    {
        public void Calculate(AbilityEntity abilityEntity)
        {
            uint damageValue = 0;
            uint healingValue = 0;
            foreach (AbilityStage combatantAbilityStage in abilityEntity.GetComponent<AbilityStagesComponent>().AbilityStages)
            {
                switch (combatantAbilityStage.AbilityStageCards.AbilityEffectType)
                {
                    case AbilityEffectType.DIRECT_DAMAGE:
                    case AbilityEffectType.RETALIATION:
                        damageValue += combatantAbilityStage.AbilityStageCards.Value;
                        break;
                    case AbilityEffectType.HEALING:
                        healingValue += combatantAbilityStage.AbilityStageCards.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(combatantAbilityStage.AbilityStageCards.AbilityEffectType));
                }
            }
            
            abilityEntity.AddComponent(new AbilityDamageComponent { TotalDamage = damageValue });
            abilityEntity.AddComponent(new AbilityHealingComponent { TotalHealing = healingValue });
        }
    }
}