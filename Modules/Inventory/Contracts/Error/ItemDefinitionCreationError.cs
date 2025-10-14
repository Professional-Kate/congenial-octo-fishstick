using IdelPog.Core.Contracts;
using IdelPog.Inventory.Contracts.Command;

namespace IdelPog.Inventory.Contracts.Error
{
    public readonly record struct ItemDefinitionCreationError
    {
        public required ItemDefinitionCreation[] ItemDefinitionCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}