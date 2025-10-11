using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Error
{
    public readonly record struct ResourceLootCreationError
    {
        public required ResourceLootCreation[] HarvestNodeLootCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}