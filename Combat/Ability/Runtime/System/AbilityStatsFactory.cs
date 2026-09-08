using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;

namespace IdelPog.Combat.Ability.Runtime.System
{
    public sealed class AbilityStatsFactory : IAbilityStatsFactory
    {
        public StatComponent[] CreateStats(AbilityStagesComponent abilityStagesComponent, AbilityCard abilityCard)
        {
            uint damageValue = 0;
            uint retaliationValue = 0;
            uint healingValue = 0;
            uint castTime = 0;
            foreach (AbilityStage combatantAbilityStage in abilityStagesComponent.AbilityStages)
            {
                checked
                {
                    castTime += combatantAbilityStage.AbilityStageCard.CastTime;
                    switch (combatantAbilityStage.AbilityStageCard.AbilityEffectType)
                    {
                        case AbilityEffectType.DIRECT_DAMAGE:
                            damageValue += combatantAbilityStage.AbilityStageCard.Value;
                            break;
                        case AbilityEffectType.HEALING:
                            healingValue += combatantAbilityStage.AbilityStageCard.Value;
                            break;
                        case AbilityEffectType.RETALIATION:
                            retaliationValue += combatantAbilityStage.AbilityStageCard.Value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(combatantAbilityStage.AbilityStageCard.AbilityEffectType));
                    }
                }
            }

            StatComponent[] statComponents =
            [
                new() { StatType = StatType.COOLDOWN, Stat = abilityCard.Cooldown },
                new() { StatType = StatType.CAST_TIME, Stat = castTime },
                new() { StatType = StatType.ABILITY_DAMAGE, Stat = damageValue },
                new() { StatType = StatType.RETALIATION_DAMAGE, Stat = retaliationValue },
                new() { StatType = StatType.ABILITY_HEALING, Stat = healingValue },
                new() { StatType = StatType.ABILITY_SLOTS, Stat = abilityCard.AbilitySlots }
            ];

            return statComponents;
        }
    }
}