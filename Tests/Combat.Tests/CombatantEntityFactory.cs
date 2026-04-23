using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime;
using IdelPog.Core.Contracts;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;

namespace IdelPog.Combat.Tests
{
    internal static class CombatantEntityFactory
    {
        private static readonly RepositoryAsserter _repositoryAsserter = new(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());

        internal static CombatantEntity CreateCombatantEntity(byte entityID, bool isFriendly = true)
        {
            CombatantCard combatantCard = new()
            {
                CombatantType = CombatantType.GOBLIN, TargetingType = TargetingType.HIGH_ATTACK,
                StatCard = new StatCard { Attack = 4, Health = 50, Speed = 1 },
                Information = new Information() { Name = nameof(CombatantType.GOBLIN), Description = "A guy!" }
            };
            
            CombatantEntity combatantEntity = new(_repositoryAsserter, combatantCard)
            {
                CombatantID = entityID,
                IsFriendly = isFriendly
            };
            
            return combatantEntity;
        }

        internal static CombatantEntity CreateCombatantEntity(byte entityID, bool isFriendly, StatCard statCard)
        {
            CombatantCard combatantCard = new()
            {
                CombatantType = CombatantType.GOBLIN,TargetingType = TargetingType.HIGH_ATTACK,
                StatCard = statCard,
                Information = new Information() { Name = nameof(CombatantType.GOBLIN), Description = "A guy!" }
            };
            
            return CreateCombatantEntity(entityID, isFriendly, combatantCard);
        }

        internal static CombatantEntity CreateCombatantEntity(byte entityID, bool isFriendly, CombatantCard combatantCard)
        {
            CombatantEntity combatantEntity = new(_repositoryAsserter, combatantCard)
            {
                CombatantID = entityID,
                IsFriendly = isFriendly
            };
            
            return combatantEntity;
        }
    }
}