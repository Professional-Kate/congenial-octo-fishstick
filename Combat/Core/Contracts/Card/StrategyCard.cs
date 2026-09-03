using IdelPog.Combat.Core.Contracts.Enum;

namespace IdelPog.Combat.Core.Contracts.Card
{
    public readonly record struct StrategyCard
    {
        public required TargetingPreference TargetingPreference { get; init; }
        public required CombatantStatType CombatantStatType { get; init; }
        public required TargetingType TargetingType { get; init; }
        public required byte Priority { get; init; }
    }
}