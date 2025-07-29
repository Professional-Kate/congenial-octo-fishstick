using IdelPog.Common.Responses;

namespace IdelPog.Common.Errors
{
    public record SkillChangeError
    {
        public required SkillChangeResponse SkillChangeResponse { get; init; }
        public required BaseError BaseError { get; init; }
    }
}