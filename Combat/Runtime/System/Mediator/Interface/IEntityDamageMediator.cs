using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Mediator.Interface
{
    public interface IEntityDamageMediator
    {
        public void ApplyDamage(CombatantEntity targetCombatant, CombatantEntity attackingCombatant, CombatantAbilityEntity attackingCombatantAbility);
    }
}