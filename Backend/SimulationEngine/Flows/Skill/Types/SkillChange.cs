namespace IdelPog.SimulationEngine.Flows.Skill
{
    public readonly record struct SkillChange
    {
        public required SkillID SkillID { get; init; }
    }
}