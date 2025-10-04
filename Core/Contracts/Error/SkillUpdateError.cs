using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Error
{
    public readonly record struct SkillUpdateError
    {
        public required SkillUpdate[] SkillUpdates { get; init; }
        public required BaseError BaseError { get; init; }
    }
}