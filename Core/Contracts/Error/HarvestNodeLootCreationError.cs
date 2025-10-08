using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Error
{
    public readonly record struct HarvestNodeLootCreationError
    {
        public required HarvestNodeLootCreation[] HarvestNodeLootCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}