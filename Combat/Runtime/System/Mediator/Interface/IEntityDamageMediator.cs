using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Mediator.Interface
{
    public interface IEntityDamageMediator
    {
        public void ApplyDamage(IEnumerable<CombatantEntity> targetCombatants, CombatantEntity attackingCombatant, CombatantAbilityEntity attackingCombatantAbility, double tick);
    }
}