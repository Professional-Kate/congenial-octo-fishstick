using IdelPog.Common.Enums;

namespace IdelPog.Common.Commands
{
    public readonly record struct SetSkill
    {
        public required SkillID SkillID { get; init; }
    }
}