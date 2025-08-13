using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Progression;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct SkillUpdateResponse
    {
        public required SkillID SkillID { get; init; }
        public required LevelProgress LevelProgress { get; init; }
        public required bool HasLeveled { get; init; }
    }
}