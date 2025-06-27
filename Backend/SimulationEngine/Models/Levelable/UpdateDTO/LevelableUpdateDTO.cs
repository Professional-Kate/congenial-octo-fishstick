namespace IdelPog.SimulationEngine.Models
{
    public readonly record struct LevelableUpdateDTO
    {
        public required byte Level { get; init; }
        public required int Experience { get; init; }
        public required int NextLevelExperience { get; init; }
        public required int ExperiencePerAction { get; init; }
    }
}