using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Factory.Interface
{
    public interface IAbilityEntityFactory
    {
        public AbilityEntity[] Create(EquippedAbilityDefinition equippedAbilityDefinition, byte instanceID);
    }
}