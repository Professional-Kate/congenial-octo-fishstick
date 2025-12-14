using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;

namespace IdelPog.Combat.Factory.Interface
{
    public interface IAbilityDefinitionFactory
    {
        public AbilityDefinition Create(AbilityDefinitionCreation abilityDefinitionCreation);
    }
}