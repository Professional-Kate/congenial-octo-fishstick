using IdelPog.Combat.Stat.Contracts.Enum;

namespace IdelPog.Combat.Stat.Contracts.Command
{
    public readonly record struct StatConversionCreation
    {
        public required StatType TargetStatType { get; init; }
        public required StatType SourceStatType { get; init; }
        public required int SourceStatInterval { get; init; }
        public required StatOperation StatOperation { get; init; }
        public required double TargetStatModifier { get; init; }
    }
}