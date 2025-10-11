using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Contracts.Command
{
    public readonly record struct HarvestNodeUnlock
    {
        public required SkillID SkillID { get; init; }
        public required byte SkillLevel { get; init; }
    }
}