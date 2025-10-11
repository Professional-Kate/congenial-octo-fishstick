using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Contracts.Command
{
    public readonly record struct LocationLootCreation
    {
        public required LocationID LocationID { get; init; }
        public required ResourceID ResourceID { get; init; }
        public required LootTableEntry[] LootTableEntries { get; init; }
        public required GrantPolicyEntry GrantPolicyEntry { get; init; }
    }
}