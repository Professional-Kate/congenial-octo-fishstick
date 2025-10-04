using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Error
{
    public readonly record struct HarvestNodeUpdateError
    {
        public required HarvestNodeUpdate[] HarvestNodeUpdates { get; init; }
        public required BaseError BaseError { get; init; }
    }
}