using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Structures.Level;

namespace IdelPog.SimulationEngine.Skill
{
    public readonly record struct SkillUpdateResponse
    {
        public required SkillID SkillID { get; init; }
        public required LevelProgress LevelProgress { get; init; }
        public required bool HasLeveled { get; init; }
    }
}