using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Error
{
    public readonly record struct LocationLootCreationError
    {
        public required LocationLootCreation[] LocationLootCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}