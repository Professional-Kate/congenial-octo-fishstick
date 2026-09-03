using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Combatant.Contracts.Command
{
    public readonly record struct CombatantCreation
    { 
        public required CombatantType CombatantType { get; init; }
        public required StatCard StatCard { get; init; }
        public required AgilityCard AgilityCard { get; init; }
    }
}