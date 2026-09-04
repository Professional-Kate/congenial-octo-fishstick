using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Entities;

namespace IdelPog.Combat.Ability.Runtime.System.Interface
{
    public interface IAbilityEntityFactory
    {
        public AbilityEntity[] Create(EquippedAbilityDefinition equippedAbilityDefinition, byte instanceID);
    }
}