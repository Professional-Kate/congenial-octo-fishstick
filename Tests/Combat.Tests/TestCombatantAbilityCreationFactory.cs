using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Tests
{
    internal static class TestCombatantAbilityCreationFactory
    {
        public static CombatantAbilityCreation Create(AbilityType abilityType)
        {
            return Create(abilityType, 25, 50);
        }
        
        public static CombatantAbilityCreation Create(AbilityType abilityType, uint speed, uint damage)
        {
            return new CombatantAbilityCreation
            {
                Information = new Information { Name = "", Description = "" },
                AbilityType = abilityType,
                Speed = speed,
                Damage = damage
            };
        }
    }
}