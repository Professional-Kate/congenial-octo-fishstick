using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Contracts.Response
{
    public readonly record struct LocationLootCreationResponse
    {
        public required LocationID LocationID { get; init; }
        public required LootTableEntry[] LootTableEntries { get; init; }
    }
}