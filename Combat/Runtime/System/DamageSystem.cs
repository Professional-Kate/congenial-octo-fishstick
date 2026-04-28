using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class DamageSystem : IDamageSystem
    {
        public uint DealDamage(CombatantEntity targetCombatant, uint attackerAttack, CombatantAbilityEntity attackerAbility)
        {
            CombatantStatsComponent targetStats = targetCombatant.GetComponent<CombatantStatsComponent>();
            DamageComponent damageComponent = attackerAbility.GetComponent<DamageComponent>();
            
            uint newHealth = CalculateNewHealth(targetStats.Health, attackerAttack, damageComponent.Damage);
            targetCombatant.UpdateCombatantStats(targetStats with { Health = newHealth });
            
            return newHealth;
        }
        
        private static uint CalculateNewHealth(uint defenderHealth, uint attackerAttack, uint abilityDamage)
        {
            uint calculatedDamage = attackerAttack + abilityDamage;
            if (defenderHealth <= calculatedDamage)
            {
                return 0;
            }

            return defenderHealth - calculatedDamage;
        }
    }
}