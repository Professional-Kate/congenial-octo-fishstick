using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Inventory
{
    public readonly record struct InventoryUpdate
    {
        public ItemID ItemID { get; init; }
        public uint Amount { get; init; }
        public ActionType Action { get; init; }
    }
}