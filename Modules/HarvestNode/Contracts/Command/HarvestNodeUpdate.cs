using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Contracts.Command
{
    public readonly record struct HarvestNodeUpdate
    {
        public required ResourceID ResourceID { get; init; }
        public required SkillID SkillID { get; init; }
    }
}