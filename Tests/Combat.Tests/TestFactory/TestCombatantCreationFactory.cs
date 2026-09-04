using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts.Enum;
using IdelPog.Combat.Core.Contracts.Card;

namespace IdelPog.Combat.Tests.TestFactory
{
    public static class TestCombatantCreationFactory
    {
        public static CombatantCreation CreateCombatantCreation(CombatantType combatantType, HealthCard healthCard, AgilityCard agilityCard)
        {
            return new CombatantCreation
            {
                CombatantType = combatantType,
                HealthCard = healthCard,
                AgilityCard = agilityCard
            };
        }
        
        public static CombatantCreation CreateCombatantCreation(CombatantType combatantType, HealthCard healthCard)
        {
            return CreateCombatantCreation(combatantType, healthCard, new AgilityCard { Speed = 1u, Initiative = 1u });
        }
        
        public static CombatantCreation CreateCombatantCreation(CombatantType combatantType)
        {
            return CreateCombatantCreation(combatantType, new HealthCard { Health = 10, BaseHealth = 10 });
        }
    }
}