using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts.Enum;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;

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
            StatsComponent statsComponent = new() { StatComponents =  CreateStatComponents(combatantCreation) };
            CombatantEntity combatantEntity = new(statsComponent)
            {
                CombatantID =  combatantID,
                InstanceID = combatantID,
                CombatantType = combatantCreation.CombatantType,
                TargetingType = targetingType
            };
            
            return combatantEntity;
        }
        
        private static StatComponent[] CreateStatComponents(CombatantCreation combatantCreation)
        {
            return
            [
                new StatComponent { StatType = StatType.BASE_HEALTH, Stat = combatantCreation.HealthCard.BaseHealth },
                new StatComponent { StatType = StatType.HEALTH, Stat = combatantCreation.HealthCard.Health },
                new StatComponent { StatType = StatType.SPEED, Stat = combatantCreation.AgilityCard.Speed },
                new StatComponent { StatType = StatType.INITIATIVE, Stat = combatantCreation.AgilityCard.Initiative }
            ];
        }
    }
}