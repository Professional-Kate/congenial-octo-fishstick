using IdelPog.Common.Enums;

namespace IdelPog.Common.Commands
{
    public readonly record struct NodeCreation
    {
        public required ResourceID[] ResourceIDs { get; init; }
        public required SkillID LinkedSkill { get; init; }
    }
}