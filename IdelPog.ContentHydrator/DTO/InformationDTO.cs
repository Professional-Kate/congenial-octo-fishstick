namespace ContentHydrator.DTO
{
    public sealed record InformationDTO
    {
        public required string Name { get; init; }
        
        public required string Description { get; init; }
    }
}