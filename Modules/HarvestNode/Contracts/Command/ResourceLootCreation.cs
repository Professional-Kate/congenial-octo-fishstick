using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Contracts.Command
{
    public readonly record struct ResourceLootCreation
    {
        public required ResourceID ResourceID { get; init; }
        public required LootTableEntry[] LootTableEntries { get; init; }
        public required GrantPolicyEntry GrantPolicyEntry { get; init; }
    }
}