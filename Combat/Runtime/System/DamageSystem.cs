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
            
            uint newHealth = CalculateNewHealth(targetStats.Health, GetCalculatedDamage(attackerAttack, attackerAbility));
            targetCombatant.UpdateCombatantStats(targetStats with { Health = newHealth });
            
            return newHealth;
        }

        public uint GetCalculatedDamage(uint attackerAttack, CombatantAbilityEntity attackerAbility)
        {
            DamageComponent damageComponent = attackerAbility.GetComponent<DamageComponent>();
            
            return attackerAttack + damageComponent.PhysicalDamage;
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