using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Tests
{
    public static class CombatantCardFactory
    {
        public static CombatantCard CreateCombatantCard(CombatantType combatantType, StatCard statCard, Information information, params AbilityCard[] skillCards)
        {
            return new CombatantCard
            {
                CombatantType = combatantType,
                Information = information,
                StatCard = statCard,
                AbilityCards = skillCards
            };
        }
        
        public static CombatantCard CreateCombatantCard(CombatantType combatantType, StatCard statCard, Information information)
        {
            return CreateCombatantCard(combatantType, statCard, information, new AbilityCard { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { TargetingType = TargetingType.HIGH_ATTACK }});
        }

        public static CombatantCard CreateCombatantCard(CombatantType combatantType, StatCard statCard)
        {
            return CreateCombatantCard(combatantType, statCard, new Information { Name = "", Description = "" });
        }
        
        public static CombatantCard CreateCombatantCard(CombatantType combatantType)
        {
            return CreateCombatantCard(combatantType, new StatCard { Health = 10, Attack = 5, Speed = 5 });
        }
    }
}