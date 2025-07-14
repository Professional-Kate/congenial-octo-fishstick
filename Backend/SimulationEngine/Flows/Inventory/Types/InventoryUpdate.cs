using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Structures;

namespace IdelPog.SimulationEngine.Inventory
{
    public readonly record struct InventoryUpdate
    {
        public ItemID ItemID { get; init; }
        public int Amount { get; init; }
        public ActionType Action { get; init; }
    }
}