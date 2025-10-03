using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Command
{
    public readonly record struct HarvestNodeUnlock
    {
        public required SkillID SkillID { get; init; }
        public required byte SkillLevel { get; init; }
    }
}