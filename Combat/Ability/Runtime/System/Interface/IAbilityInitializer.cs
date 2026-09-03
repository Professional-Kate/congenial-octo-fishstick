using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Entities;

namespace IdelPog.Combat.Ability.Runtime.System.Interface
{
    public interface IAbilityInitializer
    {
        public void InitializeAbilities(CombatantEntity combatantEntity, IReadOnlyList<AbilityEntity> combatantAbilityEntities);
    }
}