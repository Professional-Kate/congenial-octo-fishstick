using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Tests
{
    internal static class TestAbilityCreationFactory
    {
        public static AbilityCreation Create(AbilityType abilityType)
        {
            return Create(abilityType, 25, 50);
        }
        
        public static AbilityCreation Create(AbilityType abilityType, uint speed, uint damage)
        {
            return new AbilityCreation
            {
                Information = new Information { Name = "", Description = "" },
                AbilityType = abilityType,
                Speed = speed,
                Damage = damage
            };
        }
    }
}