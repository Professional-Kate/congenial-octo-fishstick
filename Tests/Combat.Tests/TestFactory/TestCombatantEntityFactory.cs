using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts.Enum;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;

namespace IdelPog.Combat.Tests.TestFactory
{
    internal static class TestCombatantEntityFactory
    {
        internal static CombatantEntity Create(byte combatantID, TargetingType targetingType)
        {
            return Create(combatantID, targetingType, new HealthCard { Health = 50, BaseHealth = 50 });
        }
        
        internal static CombatantEntity Create(byte combatantID, TargetingType targetingType, HealthCard healthCard)
        {
            CombatantCreation combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.GOBLIN, healthCard, new AgilityCard { Speed = 15, Initiative = 1 });
            
            return Create(combatantID, targetingType, combatantCreation);
        }
        
        internal static CombatantEntity Create(byte combatantID, TargetingType targetingType, AgilityCard agilityCard)
        {
            CombatantCreation combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.GOBLIN, new HealthCard { Health = 50, BaseHealth = 50 }, agilityCard);
            
            return Create(combatantID, targetingType, combatantCreation);
        }

        internal static CombatantEntity Create(byte combatantID, TargetingType targetingType, CombatantCreation combatantCreation)
        {
            CombatantEntity combatantEntity = new(combatantCreation.HealthCard, combatantCreation.AgilityCard)
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