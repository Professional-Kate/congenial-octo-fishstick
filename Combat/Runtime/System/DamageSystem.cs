using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class DamageSystem : IDamageSystem
    {
        public uint DealDamage(CombatantEntity targetCombatant, uint attack)
        {
            CombatantStatsComponent targetStats = targetCombatant.GetComponent<CombatantStatsComponent>();
            
            uint newHealth = CalculateNewHealth(targetStats.Health, attack);
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