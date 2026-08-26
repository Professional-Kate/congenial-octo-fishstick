using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class AbilityEffectValueCalculator : IAbilityEffectValueCalculator
    {
        public void Calculate(CombatantAbilityEntity combatantAbilityEntity)
        {
            uint damageValue = 0;
            uint healingValue = 0;
            foreach (CombatantAbilityStage combatantAbilityStage in combatantAbilityEntity.GetComponent<AbilityStagesComponent>().AbilityStages)
            {
                switch (combatantAbilityStage.AbilityStage.AbilityEffectType)
                {
                    case AbilityEffectType.DIRECT_DAMAGE:
                    case AbilityEffectType.RETALIATION:
                        damageValue += combatantAbilityStage.AbilityStage.Value;
                        break;
                    case AbilityEffectType.HEALING:
                        healingValue += combatantAbilityStage.AbilityStage.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(combatantAbilityStage.AbilityStage.AbilityEffectType));
                }
            }
            
            combatantAbilityEntity.AddComponent(new AbilityDamageComponent { TotalDamage = damageValue });
            combatantAbilityEntity.AddComponent(new AbilityHealingComponent { TotalHealing = healingValue });
        }
    }
}