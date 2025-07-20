using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Skill
{
    public readonly record struct SkillChangeDTO
    {
        public required SkillID SkillID { get; init; }
    }
}