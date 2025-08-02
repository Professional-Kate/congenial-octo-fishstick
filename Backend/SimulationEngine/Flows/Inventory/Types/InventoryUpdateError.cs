using IdelPog.Common.Errors;

namespace IdelPog.SimulationEngine.Inventory
{
    public readonly record struct InventoryUpdateError
    {
        public required InventoryUpdate[] InventoryUpdates { get; init; }
        public required BaseError BaseError { get; init; }
    }
}