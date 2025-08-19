using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Loot.Table
{
    public readonly record struct WeightedEntry
    {
        public required ItemID ItemID { get; init; }
        public required int Weight { get; init; }
    }
}