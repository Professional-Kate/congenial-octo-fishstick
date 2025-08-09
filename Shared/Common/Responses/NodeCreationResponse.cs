using IdelPog.Common.Commands;

namespace IdelPog.Common.Responses
{
    public readonly record struct NodeCreationResponse
    {
        public required NodeCreation[] NodeCreations { get; init; }
    }
}