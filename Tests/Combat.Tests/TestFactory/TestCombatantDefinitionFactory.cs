using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;

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
                StatCard = new StatCard { Health = 10 },
                CombatantID = combatantID
            };
        }
    }
}