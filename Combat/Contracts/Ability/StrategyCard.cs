using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Contracts.Ability
{
    public readonly record struct StrategyCard
    {
        public required TargetingType TargetingType { get; init; }
    }
}