using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Currency.Contracts.Command
{
    public readonly record struct ItemBuy
    {
        public required CurrencyType CurrencyType { get; init; }
        public required ItemID ItemID { get; init; }
        public required uint Price { get; init; }
        public required uint Amount { get; init; }
    }
}