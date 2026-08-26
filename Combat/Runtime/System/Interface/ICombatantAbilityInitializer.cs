using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface ICombatantAbilityInitializer
    {
        public void InitializeAbilities(CombatantEntity combatantEntity, IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities);
    }
}