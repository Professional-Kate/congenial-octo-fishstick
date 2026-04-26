using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Factory.Interface
{
    public interface ICombatantAbilityEntityFactory
    {
        public IReadOnlyList<CombatantAbilityEntity> Create(CombatantAbilityEquip combatantAbilityEquip);
    }
}