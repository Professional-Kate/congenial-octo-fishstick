using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Command
{
    public readonly record struct HarvestNodeLootCreation
    {
        public required ItemID ItemID { get; init; }
        public required ResourceID ResourceID { get; init; }
        public required LootTableEntry[] LootTableEntries { get; init; }
    }
}