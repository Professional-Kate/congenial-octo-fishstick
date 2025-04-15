namespace ContentHydrator.DTO
{
    public sealed class LevelableDTO
    {
        public byte Level { get; init; } = 0;
        public byte Experience { get; init; } = 0;
        public byte NextLevelExperience { get; init; } = 0;
        public byte ExperiencePerAction { get; init; } = 0;
    }
}