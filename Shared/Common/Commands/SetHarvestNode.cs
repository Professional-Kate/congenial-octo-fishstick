using IdelPog.Common.Enums;

namespace IdelPog.Common.Commands
{
    public readonly record struct SetHarvestNode
    {
        public required SkillID SkillID { get; init; }
        public required ResourceID ResourceID { get; init; }
    }
}