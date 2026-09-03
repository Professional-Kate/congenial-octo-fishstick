using IdelPog.Combat.Ability.Runtime.Entity;
using IdelPog.Combat.Combatant.Runtime.Entity;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface IAbilityInitializer
    {
        public void InitializeAbilities(CombatantEntity combatantEntity, IReadOnlyList<AbilityEntity> combatantAbilityEntities);
    }
}