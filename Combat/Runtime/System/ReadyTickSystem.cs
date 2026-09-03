using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Runtime.System
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