using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Service.Interface
{
    public interface IEntityDamageService
    {
        public void ApplyDamage(IEnumerable<CombatantEntity> targetCombatants, CombatantEntity attackingCombatant, CombatantAbilityEntity attackingCombatantAbility, double tick);
    }
}