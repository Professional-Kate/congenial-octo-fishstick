using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Contracts.Response
{
    public readonly record struct LocationLootCreationResponse
    {
        public required ResourceID ResourceID { get; init; }
        public required LocationID LocationID { get; init; }
        public required LootTableEntry[] LootTableEntries { get; init; }
    }
}