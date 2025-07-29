using IdelPog.Common.DTO.Error;

namespace IdelPog.SimulationEngine.Inventory
{
    public readonly record struct InventoryUpdateErrorDTO
    {
        public required InventoryUpdate[] InventoryUpdates { get; init; }
        public required ErrorDTO ErrorDTO { get; init; }
    }
}