using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;

namespace IdelPog.Combat.Combatant.Runtime.System
{
    public sealed class DamageCalculator : IDamageCalculator
    {
        public uint DealDamage(CombatantEntity targetCombatant, AbilityStage abilityStage)
        {
            uint newHealth = CalculateNewHealth(targetCombatant.GetStat(StatType.HEALTH), GetCalculatedDamage(abilityStage));
            targetCombatant.GetComponent<StatsComponent>().ReplaceStat(StatType.HEALTH, newHealth);
            
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