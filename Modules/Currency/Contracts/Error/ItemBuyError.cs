using IdelPog.Core.Contracts;
using IdelPog.Currency.Contracts.Command;

namespace IdelPog.Currency.Contracts.Error
{
    public readonly record struct ItemBuyError
    {
        public required ItemBuy[] ItemBuys { get; init; }
        public required BaseError BaseError { get; init; }
    }
}