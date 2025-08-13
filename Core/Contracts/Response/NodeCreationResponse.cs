using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct NodeCreationResponse
    {
        public required NodeCreation[] NodeCreations { get; init; }
    }
}