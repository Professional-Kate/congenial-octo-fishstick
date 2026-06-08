using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Service.Interface
{
    public interface IEntityHealingService
    {
        public void ApplyHealing(IEnumerable<CombatantEntity> targetCombatants, CombatantEntity healingCombatant, CombatantAbilityEntity healingAbility, double tick);
    }
}