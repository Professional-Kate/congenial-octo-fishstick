using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;

namespace IdelPog.Combat.Tests
{
    internal static class TestCombatantAbilityEntityFactory
    {
        private static readonly RepositoryAsserter _repositoryAsserter = new(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());
        
        internal static CombatantAbilityEntity Create(byte combatantID, AbilityType abilityType)
        {
            return new CombatantAbilityEntity(_repositoryAsserter) { CombatantID = combatantID, AbilityType = abilityType };
        }

        internal static CombatantAbilityEntity CreateWithBaseComponents(byte combatantID, AbilityType abilityType)
        {
            CombatantAbilityEntity combatantAbilityEntity = Create(combatantID, abilityType);
            
            combatantAbilityEntity.AddComponent(new CooldownComponent { Cooldown = 1 });
            
            return combatantAbilityEntity;
        }
    }
}