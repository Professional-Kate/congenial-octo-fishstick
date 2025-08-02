using IdelPog.Common.Enums;
using IdelPog.Common.Structures;

namespace IdelPog.Common.Responses
{
    public readonly record struct SkillUpdateResponse
    {
        public required SkillID SkillID { get; init; }
        public required LevelProgress LevelProgress { get; init; }
        public required bool HasLeveled { get; init; }
    }
}