using IdelPog.Core.Contracts;
using IdelPog.Inventory.Crafting.Contracts.Command;

namespace IdelPog.Inventory.Crafting.Contracts.Error
{
    public readonly record struct RecipeCreationError
    {
        public required RecipeCreation[] RecipeCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}