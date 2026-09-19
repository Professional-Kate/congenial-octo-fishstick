using IdelPog.Combat.Ability.Contracts;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts.Enum;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Runtime.Component;
using IdelPog.Combat.Stat.Service;

namespace IdelPog.Combat.Tests.TestFactory
{
    internal static class TestCombatantEntityFactory
    {
        internal static CombatantEntity Create(byte combatantID, TargetingType targetingType)
        {
            return Create(combatantID, targetingType, new HealthCard { Health = 50, BaseHealth = 50 });
        }

        private static CombatantEntity Create(byte combatantID, TargetingType targetingType, HealthCard healthCard)
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
            StatConfiguration statConfiguration = new()
            {
                CombatantStatLinks = new CombatantStatLinks
                {
                    BaseHealthID = 0,
                    HealthID = 1,
                    SpeedID = 2,
                    InitiativeID = 3
                },
                AbilityStatLinks = new AbilityStatLinks
                {
                    AbilityDamageID = 4,
                    AbilityHealingID = 5,
                    RetaliationDamageID = 6,
                    CastTimeID = 7,
                    CooldownID = 8,
                    AbilitySlotsID = 9
                }
            };

            StatConfigurationSystem statConfigurationSystem = new();
            statConfigurationSystem.SetStatConfiguration(statConfiguration);
            
            StatsComponent statsComponent = new() { StatComponents =  CreateStatComponents(combatantCreation) };
            CombatantEntity combatantEntity = new(statsComponent, statConfigurationSystem)
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
                new StatComponent { StatID = 0, Value = combatantCreation.HealthCard.BaseHealth },
                new StatComponent { StatID = 1, Value = combatantCreation.HealthCard.Health },
                new StatComponent { StatID = 2, Value = combatantCreation.AgilityCard.Speed },
                new StatComponent { StatID = 3, Value = combatantCreation.AgilityCard.Initiative }
            ];
        }
    }
}