using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct HarvestNodeCreationResponse
    {
        public required HarvestNodeCreation[] NodeCreations { get; init; }
    }
}