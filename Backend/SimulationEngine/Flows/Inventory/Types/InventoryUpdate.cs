using IdelPog.SimulationEngine.Structures.Enums;

namespace IdelPog.SimulationEngine.Flows.Inventory
{
    public readonly record struct InventoryUpdate
    {
        public InventoryID InventoryID { get; init; }
        public int Amount { get; init; }
        public ActionType Action { get; init; }
    }
}