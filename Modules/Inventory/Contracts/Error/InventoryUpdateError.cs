using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;

namespace IdelPog.Inventory.Contracts.Error
{
    public readonly record struct InventoryUpdateError
    {
        public required InventoryUpdate[] InventoryUpdates { get; init; }
        public required BaseError BaseError { get; init; }
    }
}