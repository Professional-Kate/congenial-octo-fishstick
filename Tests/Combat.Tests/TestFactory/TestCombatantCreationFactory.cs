using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;

namespace IdelPog.Combat.Tests.TestFactory
{
    public static class TestCombatantCreationFactory
    {
        public static CombatantCreation CreateCombatantCreation(CombatantType combatantType, StatCard statCard, AgilityCard agilityCard)
        {
            return new CombatantCreation
            {
                CombatantType = combatantType,
                StatCard = statCard,
                AgilityCard = agilityCard
            };
        }
        
        public static CombatantCreation CreateCombatantCreation(CombatantType combatantType, StatCard statCard)
        {
            return CreateCombatantCreation(combatantType, statCard, new AgilityCard { Speed = 1u, Initiative = 1u });
        }
        
        public static CombatantCreation CreateCombatantCreation(CombatantType combatantType)
        {
            return CreateCombatantCreation(combatantType, new StatCard { Health = 10 });
        }
    }
}