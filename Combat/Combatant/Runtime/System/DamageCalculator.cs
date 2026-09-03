using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;

namespace IdelPog.Combat.Combatant.Runtime.System
{
    public sealed class DamageCalculator : IDamageCalculator
    {
        public uint DealDamage(CombatantEntity targetCombatant, AbilityStage abilityStage)
        {
            HealthComponent targetHealth = targetCombatant.GetComponent<HealthComponent>();
            
            uint newHealth = CalculateNewHealth(targetHealth.Health, GetCalculatedDamage(abilityStage));
            targetCombatant.ReplaceComponent(new HealthComponent { Health = newHealth });
            
            return newHealth;
        }

        public uint GetCalculatedDamage(AbilityStage abilityStage)
        {
            return abilityStage.AbilityStageCards.Value;
        }

        private static uint CalculateNewHealth(uint defenderHealth, uint calculatedDamage)
        {
            if (defenderHealth <= calculatedDamage)
            {
                return 0;
            }

            return defenderHealth - calculatedDamage;
        }
    }
}