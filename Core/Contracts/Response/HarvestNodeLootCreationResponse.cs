using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct HarvestNodeLootCreationResponse
    {
        public required ItemID ItemID { get; init; }
        public required LootTableEntry[] LootTableEntries { get; init; }
    }
}