using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Card
{
    public readonly record struct CombatantCard
    { 
        public required Information Information { get; init; }
        public required CombatantType CombatantType { get; init; }
        public required StatCard StatCard { get; init; }
        public required AbilityCard[] AbilityCards { get; init; }
    }
}