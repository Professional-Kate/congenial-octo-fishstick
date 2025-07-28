namespace IdelPog.Common.DTO.Error
{
    public record SkillChangeErrorDTO
    {
        public required SkillChangeDTO SkillChangeDTO { get; init; }
        public required ErrorDTO ErrorDTO { get; init; }
    }
}