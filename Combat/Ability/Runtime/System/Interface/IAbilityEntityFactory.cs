using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Model;

namespace IdelPog.Combat.Ability.Runtime.System.Interface
{
    public interface IAbilityEntityFactory
    {
        public AbilityEntity[] Create(EquippedAbilityDefinition equippedAbilityDefinition, byte instanceID);
    }
}