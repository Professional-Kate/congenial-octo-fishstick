namespace ContentHydrator.DTO
{
    public sealed class JobDTO
    {
        public InformationDTO Information { get; init; } = new();
        public LevelableDTO Levelable { get; init; } = new();
        public string JobID { get; init; } = string.Empty;
    }
}