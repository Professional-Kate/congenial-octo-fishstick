using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Error
{
    public readonly record struct NodeCreationError
    {
        public required NodeCreation[] NodeCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}