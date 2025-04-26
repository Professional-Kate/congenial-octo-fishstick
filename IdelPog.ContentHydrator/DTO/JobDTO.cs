namespace ContentHydrator.DTO
{
    public sealed record JobDTO
    {
        public required string JobID { get; init; }
        
        public required InformationDTO Information { get; init; }
    }
}