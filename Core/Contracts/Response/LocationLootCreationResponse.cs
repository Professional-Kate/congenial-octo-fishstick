using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct LocationLootCreationResponse
    {
        public required ResourceID ResourceID { get; init; }
        public required LocationID LocationID { get; init; }
        public required LootTableEntry[] LootTableEntries { get; init; }
    }
}