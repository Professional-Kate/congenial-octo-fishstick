using IdelPog.Combat.Ability.Runtime.Entity;
using IdelPog.Combat.Combatant.Model;

namespace IdelPog.Combat.Runtime.System.Factory.Interface
{
    public interface IAbilityEntityFactory
    {
        public AbilityEntity[] Create(EquippedAbilityDefinition equippedAbilityDefinition, byte instanceID);
    }
}