namespace IdelPog.ContentHydrator.DTO
{
    public sealed record SkillDTO
    {
        public required string SkillID { get; init; }
        
        public required InformationDTO Information { get; init; }
    }
}