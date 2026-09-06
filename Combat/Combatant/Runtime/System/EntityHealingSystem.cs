using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;

namespace IdelPog.Combat.Combatant.Runtime.System
{
    public sealed class EntityHealingSystem : IEntityHealingSystem
    {
        public void ApplyHealing(IEnumerable<CombatantEntity> targetCombatants, CombatantEntity healingCombatant, AbilityStage abilityStage, double tick)
        {
            foreach (CombatantEntity targetCombatant in targetCombatants)
            {
                UpdateHealth(targetCombatant, abilityStage.AbilityStageCards.Value);
            }
        }

        private static void UpdateHealth(CombatantEntity targetCombatant, uint healPower)
        {
            uint health = targetCombatant.GetStat(StatType.HEALTH);
            
            uint healAmount = Math.Min(healPower, GetMissingHealth(targetCombatant.GetStat(StatType.BASE_HEALTH), health));
            targetCombatant.GetComponent<StatsComponent>().ReplaceStat(StatType.HEALTH, health + healAmount);
        }

        private static uint GetMissingHealth(uint baseHealth, uint currentHealth)
        {
            if (currentHealth > baseHealth)
            {
                return 0;
            }
            
            return baseHealth - currentHealth;
        } 
    }
}