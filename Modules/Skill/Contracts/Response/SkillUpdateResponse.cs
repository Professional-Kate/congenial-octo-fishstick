using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Progression;

namespace IdelPog.Skill.Contracts.Response
{
    public readonly record struct SkillUpdateResponse
    {
        public required SkillID SkillID { get; init; }
        public required ReadOnlyLevelable ReadOnlyLevelable { get; init; }
        public required bool HasLeveled { get; init; }
    }
}