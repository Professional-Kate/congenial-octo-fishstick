using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts
{
    public readonly record struct InventoryUpdateEntry
    {
        public required InventoryUpdate InventoryUpdate { get; init; }
        public required ItemInfo ItemInfo { get; init; }
        public required MutateType MutateType { get; init; }
    }
}