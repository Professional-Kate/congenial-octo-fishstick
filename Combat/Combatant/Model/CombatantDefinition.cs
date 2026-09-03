using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;

namespace IdelPog.Combat.Combatant.Model
{
    public readonly record struct CombatantDefinition
    {
        public required byte CombatantID { get; init; }
        public required CombatantType CombatantType { get; init; }
        public required StatCard StatCard { get; init; }
        public required AgilityCard AgilityCard { get; init; }
    }
}