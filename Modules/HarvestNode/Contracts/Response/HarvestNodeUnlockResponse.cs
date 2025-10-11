using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Contracts.Response
{
    public readonly record struct HarvestNodeUnlockResponse
    {
        public required SkillID SkillID { get; init; }
        public required ResourceID ResourceID { get; init; }
    }
}