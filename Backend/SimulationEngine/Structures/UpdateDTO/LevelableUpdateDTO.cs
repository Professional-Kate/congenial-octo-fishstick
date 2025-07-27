namespace IdelPog.SimulationEngine.Structures
{
    public readonly record struct LevelableUpdateDTO
    {
        public required byte Level { get; init; }
        public required uint Experience { get; init; }
        public required uint NextLevelExperience { get; init; }
        public required uint ExperiencePerAction { get; init; }
    }
}