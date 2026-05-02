using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Core.Contracts;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;

namespace IdelPog.Combat.Tests
{
    internal static class TestAbilityEntityFactory
    {
        private static readonly RepositoryAsserter _repositoryAsserter = new(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());
        
        internal static AbilityEntity Create(AbilityType abilityType, byte abilitySlots)
        {
            return new AbilityEntity(_repositoryAsserter, new CooldownComponent { Cooldown = 1 }, new DamageComponent { Damage = 1 })
            {
                AbilityType = abilityType,
                AbilitySlots = abilitySlots,
                Information = new Information { Name = abilityType.ToString(), Description = "" }
            };
        }
    }
}