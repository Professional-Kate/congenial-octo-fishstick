using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Tests.TestFactory
{
    internal static class TestCombatantEntityFactory
    {
        internal static CombatantEntity CreateCombatantEntity(byte entityID, bool isFriendly = true)
        {
            return CreateCombatantEntity(entityID, isFriendly, new StatCard { Health = 50 });
        }
        
        internal static CombatantEntity CreateCombatantEntity(byte entityID, bool isFriendly, StatCard statCard)
        {
            CombatantCreation combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.GOBLIN, statCard, new AgilityCard { Speed = 15, Initiative = 1 }, new Information { Name = "Goblin", Description = entityID.ToString() });
            
            return CreateCombatantEntity(entityID, isFriendly, combatantCreation);
        }
        
        internal static CombatantEntity CreateCombatantEntity(byte entityID, bool isFriendly, AgilityCard agilityCard)
        {
            CombatantCreation combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.GOBLIN, new StatCard { Health = 50 }, agilityCard, new Information { Name = "Goblin", Description = entityID.ToString() });
            
            return CreateCombatantEntity(entityID, isFriendly, combatantCreation);
        }

        internal static CombatantEntity CreateCombatantEntity(byte entityID, bool isFriendly, CombatantCreation combatantCreation)
        {
            CombatantEntity combatantEntity = new(combatantCreation.StatCard, combatantCreation.AgilityCard)
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