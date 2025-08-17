using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Loot.Contracts
{
    public readonly record struct WeightedEntry
    {
        public required ItemID ItemID { get; init; }
        public required uint Weight { get; init; }
    }
}