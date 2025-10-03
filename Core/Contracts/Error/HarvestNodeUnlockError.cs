using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Error
{
    public readonly record struct HarvestNodeUnlockError
    {
        public required HarvestNodeUnlock[] HarvestNodeUnlocks { get; init; }
        public required BaseError BaseError { get; init; }
    }
}