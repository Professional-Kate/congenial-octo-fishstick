using IdelPog.Common.Commands;

namespace IdelPog.Common.Errors
{
    public readonly record struct NodeCreationError
    {
        public required NodeCreation[] NodeCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}