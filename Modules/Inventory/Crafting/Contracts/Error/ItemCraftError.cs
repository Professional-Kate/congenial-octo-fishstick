using IdelPog.Core.Contracts;
using IdelPog.Inventory.Crafting.Contracts.Command;

namespace IdelPog.Inventory.Crafting.Contracts.Error
{
    public readonly record struct ItemCraftError
    {
        public required ItemCraft[] ItemCrafts { get; init; }
        public required BaseError BaseError { get; init; }
    }
}