using IdelPog.Core.Contracts;
using IdelPog.Inventory.Contracts.Command;

namespace IdelPog.Inventory.Contracts.Error
{
    public readonly record struct ItemCraftError
    {
        public required ItemCraft[] ItemCrafts { get; init; }
        public required BaseError BaseError { get; init; }
    }
}