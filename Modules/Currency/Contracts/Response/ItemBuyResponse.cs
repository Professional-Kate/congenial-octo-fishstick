using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Currency.Contracts.Response
{
    public readonly record struct ItemBuyResponse
    {
        public required CurrencyType CurrencyType { get; init; }
        public required ItemID ItemID { get; init; }
        public required uint Price { get; init; }
        public required uint Amount { get; init; }
    }
}