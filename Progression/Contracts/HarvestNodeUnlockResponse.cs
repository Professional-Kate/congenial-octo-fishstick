using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Progression.Contracts
{
    public readonly record struct HarvestNodeUnlockResponse
    {
        public required ItemID ItemID { get; init; }
    }
}