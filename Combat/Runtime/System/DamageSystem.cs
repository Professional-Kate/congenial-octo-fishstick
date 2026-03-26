using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class DamageSystem : IDamageSystem
    {
        public uint DealDamage(CombatantEntity targetCombatant, StatCard attackerStats)
        {
            StatCard targetStats = targetCombatant.GetComponent<CombatantStatsComponent>().StatCard;
            
            uint newHealth = CalculateNewHealth(targetStats.Health, attackerStats.Attack);
            targetCombatant.UpdateCombatantStats(targetStats with { Health = newHealth });
            
            return newHealth;
        }
        
        private static uint CalculateNewHealth(uint defenderHealth, uint attackerAttack)
        {
            if (defenderHealth <= attackerAttack)
            {
                return 0;
            }

            return defenderHealth - attackerAttack;
        }
    }
}