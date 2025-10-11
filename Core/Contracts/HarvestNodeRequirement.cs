using IdelPog.Core.Contracts.Response;

namespace IdelPog.Core.Contracts
{
    public readonly record struct HarvestNodeRequirement
    {
        public required byte RequiredLevel { get; init; }
        public required HarvestNodeUnlockResponse OnUnlockCommand { get; init; }
    }
}