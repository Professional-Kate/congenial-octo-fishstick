using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Tests
{
    public static class TestCombatantCreationFactory
    {
        public static CombatantCreation CreateCombatantCreation(CombatantType combatantType, StatCard statCard, Information information, params AbilityCard[] skillCards)
        {
            return new CombatantCreation
            {
                CombatantType = combatantType,
                Information = information,
                StatCard = statCard
            };
        }
        
        public static CombatantCreation CreateCombatantCreation(CombatantType combatantType, StatCard statCard, Information information)
        {
            return CreateCombatantCreation(combatantType, statCard, information, new AbilityCard { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { TargetingType = TargetingType.HIGH_ATTACK }});
        }

        public static CombatantCreation CreateCombatantCreation(CombatantType combatantType, StatCard statCard)
        {
            return CreateCombatantCreation(combatantType, statCard, new Information { Name = "", Description = "" });
        }
        
        public static CombatantCreation CreateCombatantCreation(CombatantType combatantType)
        {
            return CreateCombatantCreation(combatantType, new StatCard { Health = 10, Attack = 5, Speed = 5 });
        }
    }
}