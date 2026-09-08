using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Ability.Service.Interface;
using IdelPog.Combat.Stat.Contracts.Enum;

namespace IdelPog.Combat.Ability.Runtime.System
{
    public sealed class ReadyTickSystem : IReadyTickSystem
    {
        private readonly ICastingCalculator _castingCalculator;

        public ReadyTickSystem(ICastingCalculator castingCalculator)
        {
            _castingCalculator = castingCalculator;
        }

        public void SetNextReadyTick(double currentTick, AbilityEntity abilityEntity, uint combatantSpeed)
        {
            double readyTick = currentTick;
            
            foreach (AbilityStage combatantAbilityStage in abilityEntity.GetComponent<AbilityStagesComponent>().AbilityStages)
            {
                if (combatantAbilityStage.AbilityStageCard.CastTime != 0)
                {
                    readyTick += _castingCalculator.GetCastDuration(combatantSpeed, combatantAbilityStage.AbilityStageCard.CastTime);
                }
            }
            
            readyTick += abilityEntity.GetStat(StatType.COOLDOWN);
            
            abilityEntity.ReplaceComponent(new ReadyTickComponent { ReadyTick = readyTick });
        }
    }
}