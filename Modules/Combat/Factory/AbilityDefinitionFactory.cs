using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Factory.Interface;

namespace IdelPog.Combat.Factory
{
    public sealed class AbilityDefinitionFactory : IAbilityDefinitionFactory
    {
        public AbilityDefinition Create(AbilityDefinitionCreation abilityDefinitionCreation)
        {
            return new AbilityDefinition
            {
                AbilityType = abilityDefinitionCreation.AbilityType,
                TargetingInformation = abilityDefinitionCreation.TargetingInformation,
                Information = abilityDefinitionCreation.Information,
                Cooldown = abilityDefinitionCreation.Cooldown,
                Damage = abilityDefinitionCreation.Damage
            };
        }
    }
}