using IdelPog.Combat.Stat.Contracts.Enum;

namespace IdelPog.Combat.Stat.Contracts.Command
{
    public readonly record struct StatConversionCreation
    {
        // TODO: need to apply these conversions
        public required byte TargetStatID { get; init; }
        public required byte SourceStatID { get; init; }
        public required int SourceStatInterval { get; init; }
        public required StatOperation StatOperation { get; init; }
        public required double TargetStatModifier { get; init; }
    }
}