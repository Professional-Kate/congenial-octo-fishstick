using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class DamageSystem : IDamageSystem
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