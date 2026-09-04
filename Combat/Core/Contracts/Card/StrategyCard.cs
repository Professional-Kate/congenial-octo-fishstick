using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Stat.Contracts.Enum;

namespace IdelPog.Combat.Core.Contracts.Card
{
    public readonly record struct StrategyCard
    {
        public required TargetingPreference TargetingPreference { get; init; }
        public required StatType StatType { get; init; }
        public required TargetingType TargetingType { get; init; }
        public required byte Priority { get; init; }
    }
}