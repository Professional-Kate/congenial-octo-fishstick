using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;

namespace IdelPog.Core.Contracts
{
    public readonly record struct HarvestNodeRequirement
    {
        public required ItemID ItemID { get; init; }
        public required byte RequiredLevel { get; init; }
        public required HarvestNodeUnlockResponse OnUnlockCommand { get; init; }
    }
}