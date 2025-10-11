using IdelPog.Core.Contracts.Error;
using IdelPog.HarvestNode.Contracts.Command;

namespace IdelPog.HarvestNode.Contracts.Error
{
    public readonly record struct HarvestNodeCreationError
    {
        public required HarvestNodeCreation[] NodeCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}