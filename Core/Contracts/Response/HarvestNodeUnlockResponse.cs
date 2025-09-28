using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct HarvestNodeUnlockResponse
    {
        public required SkillID SkillID { get; init; }
        public required ItemID ItemID { get; init; }
        public required byte SkillLevel { get; init; }
    }
}