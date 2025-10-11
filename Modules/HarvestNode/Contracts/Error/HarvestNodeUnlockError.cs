using IdelPog.Core.Contracts;
using IdelPog.HarvestNode.Contracts.Command;

namespace IdelPog.HarvestNode.Contracts.Error
{
    public readonly record struct HarvestNodeUnlockError
    {
        public required HarvestNodeUnlock[] HarvestNodeUnlocks { get; init; }
        public required BaseError BaseError { get; init; }
    }
}