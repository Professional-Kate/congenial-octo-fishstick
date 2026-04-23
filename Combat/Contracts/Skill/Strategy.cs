using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Contracts.Skill
{
    public readonly record struct Strategy
    {
        public required TargetingType TargetingType { get; init; }
    }
}