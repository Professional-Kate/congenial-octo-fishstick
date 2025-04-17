namespace ContentHydrator.DTO
{
    public readonly struct JobDTO(string jobID)
    {
        public readonly string JobID = jobID;
        public InformationDTO Information { get; init; } = new();
    }
}