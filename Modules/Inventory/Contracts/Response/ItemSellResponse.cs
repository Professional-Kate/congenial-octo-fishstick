using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Inventory.Contracts.Response
{
    public readonly record struct ItemSellResponse
    {
        public required ItemID ItemID { get; init; }
        public required uint Amount { get; init; }
    }
}