using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Combatant.Runtime.Entity;

namespace IdelPog.Combat.Service.Interface
{
    public interface IEntityHealingService
    {
        public void ApplyHealing(IEnumerable<CombatantEntity> targetCombatants, CombatantEntity healingCombatant, AbilityStage abilityStage, double tick);
    }
}