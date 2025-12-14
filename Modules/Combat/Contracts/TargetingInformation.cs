using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Contracts
{
    public readonly record struct TargetingInformation
    {
        public required TargetingType TargetingType { get; init; }
        public required byte MaxTargets { get; init; }
    }
}