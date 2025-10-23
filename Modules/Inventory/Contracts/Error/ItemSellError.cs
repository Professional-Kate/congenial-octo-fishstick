using IdelPog.Core.Contracts;
using IdelPog.Inventory.Contracts.Command;

namespace IdelPog.Inventory.Contracts.Error
{
    public readonly record struct ItemSellError
    {
        public required ItemSell[] ItemSells { get; init; }
        public required BaseError BaseError { get; init; }
    }
}