using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;

namespace IdelPog.Combat.Ability.Runtime.System
{
    public sealed class AbilityStatsFactory : IAbilityStatsFactory
    {
        private readonly IStatComponentFactory _statComponentFactory;

        public AbilityStatsFactory(IStatComponentFactory statComponentFactory)
        {
            _statComponentFactory = statComponentFactory;
        }

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
                _statComponentFactory.Create(StatType.ABILITY_SLOTS, abilityCard.AbilitySlots),
                _statComponentFactory.Create(StatType.COOLDOWN, abilityCard.Cooldown),
                _statComponentFactory.Create(StatType.CAST_TIME, castTime),
                _statComponentFactory.Create(StatType.ABILITY_DAMAGE, damageValue),
                _statComponentFactory.Create(StatType.ABILITY_HEALING, healingValue),
                _statComponentFactory.Create(StatType.RETALIATION_DAMAGE, retaliationValue)
            ];

            return statComponents;
        }
    }
}