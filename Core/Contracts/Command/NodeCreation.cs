using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Command
{
    public readonly record struct NodeCreation
    {
        public required ResourceID[] ResourceIDs { get; init; }
        public required SkillID LinkedSkill { get; init; }
    }
}