using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Command
{
    public readonly record struct CombatantCreation
    { 
        public required Information Information { get; init; }
        public required CombatantType CombatantType { get; init; }
        public required StatCard StatCard { get; init; }
        public required AgilityCard AgilityCard { get; init; }
    }
}