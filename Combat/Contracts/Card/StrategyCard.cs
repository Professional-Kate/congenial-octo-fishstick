using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Contracts.Card
{
    public readonly record struct StrategyCard
    {
        public required TargetingPreference TargetingPreference { get; init; }
        public required CombatantStatType CombatantStatType { get; init; }
    }
}