using IdelPog.Core.Contracts;
using IdelPog.HarvestNode.Contracts.Command;

namespace IdelPog.HarvestNode.Contracts.Error
{
    public readonly record struct ResourceLootCreationError
    {
        public required ResourceLootCreation[] HarvestNodeLootCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}