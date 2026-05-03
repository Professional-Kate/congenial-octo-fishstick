using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Factory.Interface
{
    public interface ICombatantAbilityFactory
    {
        public CombatantAbility CreateCombatantAbility(CombatantAbilityEntity combatantAbilityEntity);
        
        public CombatantAbility[] CreateCombatantAbilities(IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities);
    }
}