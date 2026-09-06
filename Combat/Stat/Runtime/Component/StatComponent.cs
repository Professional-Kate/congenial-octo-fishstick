using IdelPog.Combat.Stat.Contracts.Enum;

namespace IdelPog.Combat.Stat.Runtime.Component
{
    public readonly record struct StatComponent
    {
        public required StatType StatType { get; init; }
        public required uint Stat { get; init; }
    }
}