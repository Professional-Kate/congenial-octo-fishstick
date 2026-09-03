using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Combatant.Runtime.Entities;

namespace IdelPog.Combat.Combatant.Runtime.System.Interface
{
    public interface IEntityHealingSystem
    {
        public void ApplyHealing(IEnumerable<CombatantEntity> targetCombatants, CombatantEntity healingCombatant, AbilityStage abilityStage, double tick);
    }
}