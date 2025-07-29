using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Structures;

namespace IdelPog.SimulationEngine.Inventory
{
    public readonly record struct InventoryUpdateResponse
    {
        public required ItemInfo ItemInfo { get; init; }
        public required ActionType ActionType { get; init; }
        public required MutateType MutateType { get; init; }
    }
}