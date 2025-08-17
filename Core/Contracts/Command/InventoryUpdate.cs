using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Command
{
    public readonly record struct InventoryUpdate
    {
        public ItemID ItemID { get; init; }
        public uint Amount { get; init; }
        public ActionType ActionType { get; init; }
    }
}