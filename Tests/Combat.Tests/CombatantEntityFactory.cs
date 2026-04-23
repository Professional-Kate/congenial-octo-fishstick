using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.System;
using IdelPog.Core.Contracts;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;

namespace IdelPog.Combat.Tests
{
    internal static class CombatantEntityFactory
    {
        private static readonly RepositoryAsserter _repositoryAsserter = new(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());
        private static readonly SkillComponentFactory _skillComponentFactory = new();

        internal static CombatantEntity CreateCombatantEntity(byte entityID, bool isFriendly = true)
        {
            return CreateCombatantEntity(entityID, isFriendly, new StatCard { Attack = 4, Health = 50, Speed = 1 });
        }

        internal static CombatantEntity CreateCombatantEntity(byte entityID, bool isFriendly, StatCard statCard)
        {
            CombatantCard combatantCard = CombatantCardFactory.CreateCombatantCard(CombatantType.GOBLIN, statCard, new Information { Name = "Goblin", Description = "A guy!" });
            
            return CreateCombatantEntity(entityID, isFriendly, combatantCard);
        }

        internal static CombatantEntity CreateCombatantEntity(byte entityID, bool isFriendly, CombatantCard combatantCard)
        {
            CombatantEntity combatantEntity = new(_repositoryAsserter, combatantCard.StatCard, _skillComponentFactory.CreateMultiple(combatantCard.SkillCards))
            {
                CombatantID = entityID,
                IsFriendly = isFriendly,
                CombatantType = combatantCard.CombatantType,
                CombatantInformation = combatantCard.Information
            };
            
            return combatantEntity;
        }
    }
}