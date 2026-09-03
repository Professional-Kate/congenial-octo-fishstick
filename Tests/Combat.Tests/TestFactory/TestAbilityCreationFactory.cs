using IdelPog.Combat.Ability.Contracts.Command;
using IdelPog.Combat.Ability.Model;

namespace IdelPog.Combat.Tests.TestFactory
{
    internal static class TestAbilityCreationFactory
    {
        public static AbilityCreation Create(AbilityDefinition abilityDefinition)
        {
            return new AbilityCreation
            {
                AbilityCard = abilityDefinition.AbilityCard,
                TriggerCard = abilityDefinition.TriggerCard,
                AbilityStageCards = [..abilityDefinition.AbilityStages]
            };
        }
    }
}