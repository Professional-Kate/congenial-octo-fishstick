using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Ability.Service.Interface;

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
                if (combatantAbilityStage.AbilityStageCards.CastTime != 0)
                {
                    readyTick += _castingCalculator.GetCastDuration(combatantSpeed, combatantAbilityStage.AbilityStageCards.CastTime);
                }
            }
            
            CooldownComponent cooldownComponent = abilityEntity.GetComponent<CooldownComponent>();
            readyTick += cooldownComponent.Cooldown;
            
            abilityEntity.ReplaceComponent(new ReadyTickComponent { ReadyTick = readyTick });
        }
    }
}