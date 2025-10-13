using IdelPog.Core.Contracts;
using IdelPog.Inventory.Contracts.Command;

namespace IdelPog.Inventory.Contracts.Error
{
    public readonly record struct RecipeCreationError
    {
        public required RecipeCreation[] RecipeCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}