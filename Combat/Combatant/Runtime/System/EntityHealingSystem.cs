using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;

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
            BaseHealthComponent baseHealthComponent = targetCombatant.GetComponent<BaseHealthComponent>();
            HealthComponent healthComponent = targetCombatant.GetComponent<HealthComponent>();
            
            uint healAmount = Math.Min(healPower, GetMissingHealth(baseHealthComponent.Health, healthComponent.Health));
            targetCombatant.ReplaceComponent(new HealthComponent { Health = healthComponent.Health + healAmount});
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