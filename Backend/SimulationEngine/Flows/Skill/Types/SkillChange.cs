namespace IdelPog.SimulationEngine.Skill
{
    public readonly record struct SkillChange
    {
        public required SkillID SkillID { get; init; }
    }
}