using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Tests.TestFactory
{
    public static class TestCombatantCreationFactory
    {
        public static CombatantCreation CreateCombatantCreation(CombatantType combatantType, StatCard statCard, Information information, AgilityCard agilityCard, params CombatantAbilityCard[] skillCards)
        {
            return new CombatantCreation
            {
                CombatantType = combatantType,
                Information = information,
                StatCard = statCard,
                AgilityCard = agilityCard
            };
        }
        
        public static CombatantCreation CreateCombatantCreation(CombatantType combatantType, StatCard statCard, AgilityCard agilityCard, Information information)
        {
            return CreateCombatantCreation(combatantType, statCard, information, agilityCard, new CombatantAbilityCard { AbilityType = AbilityType.SLASH, StrategyCard = new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH }});
        }

        public static CombatantCreation CreateCombatantCreation(CombatantType combatantType, StatCard statCard)
        {
            return CreateCombatantCreation(combatantType, statCard, new AgilityCard { Speed = 1u, Initiative = 1u }, new Information { Name = "", Description = "" });
        }
        
        public static CombatantCreation CreateCombatantCreation(CombatantType combatantType)
        {
            return CreateCombatantCreation(combatantType, new StatCard { Health = 10 });
        }
    }
}