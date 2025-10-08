using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts
{
    public readonly record struct LootTableEntry
    {
        public required ItemID ItemID { get; init; }
        public required int Weight { get; init; }
    }
}