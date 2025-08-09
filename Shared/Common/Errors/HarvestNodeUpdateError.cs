using IdelPog.Common.Responses;

namespace IdelPog.Common.Errors
{
    public readonly record struct HarvestNodeUpdateError
    {
        public required SkillUpdateResponse SkillUpdateResponse { get; init; }
        public required BaseError BaseError { get; init; }
    }
}