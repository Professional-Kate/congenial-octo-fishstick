using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts
{
    public readonly record struct ItemInfo
    {
        public required ItemID ItemID { get; init; }
        public required uint BaseSellPrice { get; init; }
        public required uint Amount { get; init; }
    }
}