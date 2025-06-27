using IdelPog.SimulationEngine.Structures;

namespace IdelPog.SimulationEngine.Flows.Inventory
{
    public readonly record struct InventoryUpdateDTO
    {
        public required ItemDTO ItemDTO { get; init; }
        public required ActionType ActionType { get; init; }
        public required MutateType MutateType { get; init; }
    }
}