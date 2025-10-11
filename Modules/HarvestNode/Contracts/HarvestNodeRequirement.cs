using IdelPog.HarvestNode.Contracts.Response;

namespace IdelPog.HarvestNode.Contracts
{
    public readonly record struct HarvestNodeRequirement
    {
        public required byte RequiredLevel { get; init; }
        public required HarvestNodeUnlockResponse OnUnlockCommand { get; init; }
    }
}