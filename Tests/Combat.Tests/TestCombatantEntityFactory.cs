using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Tests
{
    internal static class TestCombatantEntityFactory
    {
        internal static CombatantEntity CreateCombatantEntity(byte entityID, bool isFriendly = true)
        {
            return CreateCombatantEntity(entityID, isFriendly, new StatCard { Attack = 4, Health = 50, Speed = 1 });
        }

        internal static CombatantEntity CreateCombatantEntity(byte entityID, bool isFriendly, StatCard statCard)
        {
            CombatantCreation combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.GOBLIN, statCard, new Information { Name = "Goblin", Description = "A guy!" });
            
            return CreateCombatantEntity(entityID, isFriendly, combatantCreation);
        }

        internal static CombatantEntity CreateCombatantEntity(byte entityID, bool isFriendly, CombatantCreation combatantCreation)
        {
            CombatantEntity combatantEntity = new(combatantCreation.StatCard)
            {
                CombatantID = entityID,
                CombatantType = combatantCreation.CombatantType,
                CombatantInformation = combatantCreation.Information
            };
            
            combatantEntity.AddComponent(new FriendlyStatusComponent { IsFriendly = isFriendly });
            
            return combatantEntity;
        }
    }
}