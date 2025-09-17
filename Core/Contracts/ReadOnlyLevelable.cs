namespace IdelPog.Core.Contracts
{
    public readonly record struct ReadOnlyLevelable
    {
        public required byte Level { get; init; }
        public required uint Experience { get; init; }
        public required uint NextLevelExperience { get; init; }
        public required uint ExperiencePerAction { get; init; }
    }
}