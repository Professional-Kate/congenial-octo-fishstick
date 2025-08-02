using IdelPog.Common.Commands;

namespace IdelPog.Common.Errors
{
    public readonly record struct SkillChangeError
    {
        public required SkillChange SkillChange { get; init; }
        public required BaseError BaseError { get; init; }
    }
}