namespace IdelPog.SimulationEngine.Skill
{
    public readonly record struct SkillChangeDTO
    {
        public required SkillID CurrentSkill { get; init; }
    }
}