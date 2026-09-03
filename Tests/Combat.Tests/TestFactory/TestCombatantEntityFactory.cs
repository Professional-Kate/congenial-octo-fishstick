using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Tests.TestFactory
{
    internal static class TestCombatantEntityFactory
    {
        internal static CombatantEntity Create(byte combatantID, TargetingType targetingType)
        {
            return Create(combatantID, targetingType, new StatCard { Health = 50 });
        }
        
        internal static CombatantEntity Create(byte combatantID, TargetingType targetingType, StatCard statCard)
        {
            CombatantCreation combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.GOBLIN, statCard, new AgilityCard { Speed = 15, Initiative = 1 });
            
            return Create(combatantID, targetingType, combatantCreation);
        }
        
        internal static CombatantEntity Create(byte combatantID, TargetingType targetingType, AgilityCard agilityCard)
        {
            CombatantCreation combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.GOBLIN, new StatCard { Health = 50 }, agilityCard);
            
            return Create(combatantID, targetingType, combatantCreation);
        }

        internal static CombatantEntity Create(byte combatantID, TargetingType targetingType, CombatantCreation combatantCreation)
        {
            CombatantEntity combatantEntity = new(combatantCreation.StatCard, combatantCreation.AgilityCard)
            {
                CombatantID =  combatantID,
                InstanceID = combatantID,
                CombatantType = combatantCreation.CombatantType,
                TargetingType = targetingType
            };
            
            return combatantEntity;
        }
    }
}