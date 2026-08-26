using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Tests.TestFactory
{
    internal static class TestCombatantEntityFactory
    {
        internal static CombatantEntity CreateCombatantEntity(byte combatantID, TargetingType targetingType = TargetingType.FRIENDLY)
        {
            return CreateCombatantEntity(combatantID, targetingType, new StatCard { Health = 50 });
        }
        
        internal static CombatantEntity CreateCombatantEntity(byte combatantID, TargetingType targetingType, StatCard statCard)
        {
            CombatantCreation combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.GOBLIN, statCard, new AgilityCard { Speed = 15, Initiative = 1 });
            
            return CreateCombatantEntity(combatantID, targetingType, combatantCreation);
        }
        
        internal static CombatantEntity CreateCombatantEntity(byte combatantID, TargetingType targetingType, AgilityCard agilityCard)
        {
            CombatantCreation combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.GOBLIN, new StatCard { Health = 50 }, agilityCard);
            
            return CreateCombatantEntity(combatantID, targetingType, combatantCreation);
        }

        internal static CombatantEntity CreateCombatantEntity(byte combatantID, TargetingType targetingType, CombatantCreation combatantCreation)
        {
            CombatantEntity combatantEntity = new(combatantCreation.StatCard, combatantCreation.AgilityCard)
            {
                CombatantID = combatantID,
                CombatantType = combatantCreation.CombatantType
            };
            
            combatantEntity.AddComponent(new TargetingTypeComponent { TargetingType = targetingType });
            combatantEntity.AddComponent(new CombatParticipantComponent());
            
            return combatantEntity;
        }
    }
}