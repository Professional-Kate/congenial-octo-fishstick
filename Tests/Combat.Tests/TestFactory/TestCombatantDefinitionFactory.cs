using IdelPog.Combat.Combatant.Contracts.Enum;
using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Core.Contracts.Card;

namespace IdelPog.Combat.Tests.TestFactory
{
    public static class TestCombatantDefinitionFactory
    {
        public static CombatantDefinition Create(byte combatantID, CombatantType combatantType)
        {
            return new CombatantDefinition
            {
                CombatantType = combatantType,
                AgilityCard = new AgilityCard { Speed = 10, Initiative = 5 },
                HealthCard = new HealthCard { Health = 10, BaseHealth = 10 },
                CombatantID = combatantID
            };
        }
    }
}