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

        public void SetNextReadyTick(double currentTick, CombatantAbilityEntity combatantAbilityEntity, uint combatantSpeed)
        {
            double readyTick = currentTick;
            
            foreach (CombatantAbilityStage combatantAbilityStage in combatantAbilityEntity.GetComponent<AbilityStagesComponent>().AbilityStages)
            {
                if (combatantAbilityStage.AbilityStage.CastTime != 0)
                {
                    readyTick += _castingCalculator.GetCastDuration(combatantSpeed, combatantAbilityStage.AbilityStage.CastTime);
                }
            }
            
            CooldownComponent cooldownComponent = combatantAbilityEntity.GetComponent<CooldownComponent>();
            readyTick += cooldownComponent.Cooldown;
            
            combatantAbilityEntity.ReplaceComponent(new ReadyTickComponent { ReadyTick = readyTick });
        }
    }
}