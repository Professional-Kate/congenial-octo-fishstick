using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class DamageSystem : IDamageSystem
    {
        public uint DealDamage(CombatantEntity targetCombatant, CombatantAbilityStage combatantAbilityStage)
        {
            HealthComponent targetHealth = targetCombatant.GetComponent<HealthComponent>();
            
            uint newHealth = CalculateNewHealth(targetHealth.Health, GetCalculatedDamage(combatantAbilityStage));
            targetCombatant.ReplaceComponent(new HealthComponent { Health = newHealth });
            
            return newHealth;
        }

        public uint GetCalculatedDamage(CombatantAbilityStage combatantAbilityStage)
        {
            return combatantAbilityStage.AbilityStage.Value;
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