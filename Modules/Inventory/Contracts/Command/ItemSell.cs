using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Inventory.Contracts.Command
{
    public readonly record struct ItemSell
    { 
        public required CurrencyType CurrencyType { get; init; }
        public required ItemID ItemID { get; init; }
        public required uint Amount { get; init; }
    }
}