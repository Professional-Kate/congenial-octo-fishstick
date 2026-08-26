using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Factory.Interface
{
    public interface ICombatantAbilityFactory
    {
        public byte[] GetCombatantAbilityIDs(IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities);
    }
}