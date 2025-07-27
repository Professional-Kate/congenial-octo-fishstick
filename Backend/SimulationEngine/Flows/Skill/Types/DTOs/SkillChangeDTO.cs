using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Skill
{
    public readonly record struct SkillChangeDTO
    {
        public required SkillID SkillID { get; init; }
        public required ResourceID ResourceID { get; init; }
    }
}