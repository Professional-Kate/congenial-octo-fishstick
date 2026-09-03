using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Runtime.System
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