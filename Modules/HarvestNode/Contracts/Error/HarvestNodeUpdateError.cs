using IdelPog.Core.Contracts.Error;
using IdelPog.HarvestNode.Contracts.Command;

namespace IdelPog.HarvestNode.Contracts.Error
{
    public readonly record struct HarvestNodeUpdateError
    {
        public required HarvestNodeUpdate[] HarvestNodeUpdates { get; init; }
        public required BaseError BaseError { get; init; }
    }
}