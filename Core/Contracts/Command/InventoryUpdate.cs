using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Command
{
    public readonly record struct InventoryUpdate
    {
        public required ItemID ItemID { get; init; }
        public required uint Amount { get; init; }
        public required ActionType ActionType { get; init; }
    }
}