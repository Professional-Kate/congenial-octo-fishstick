using IdelPog.SimulationEngine.Models;

namespace IdelPog.SimulationEngine.Flows.Skill
{
    public readonly record struct SkillUpdateDTO
    {
        public required SkillID SkillID { get; init; }
        public required LevelableUpdateDTO  LevelableUpdateDTO { get; init; }
        public required bool HasLeveled { get; init; }
    }
}