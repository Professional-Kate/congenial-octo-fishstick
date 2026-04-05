using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Contracts.Card
{
    public readonly record struct CombatantCard
    { 
        public required CombatantType CombatantType { get; init; }
        public required TargetingType TargetingType { get; init; }
        public required StatCard StatCard { get; init; }
    }
}