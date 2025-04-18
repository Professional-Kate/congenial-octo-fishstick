namespace ContentHydrator.DTO
{
    public sealed record LevelableDTO
    {
        public required byte Level { get; init; }
        public required byte Experience { get; init; }
        public required byte NextLevelExperience { get; init; }
        public required byte ExperiencePerAction { get; init; }
    }
}