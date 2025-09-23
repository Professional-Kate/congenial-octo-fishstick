using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Progression.Contracts
{
    public readonly record struct HarvestNodeUnlockResponse
    {
        public required byte SkillLevel { get; init; }
        public required ItemID ItemID { get; init; }
    }
}