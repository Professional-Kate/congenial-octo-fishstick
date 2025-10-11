using IdelPog.Core.Contracts.Error;
using IdelPog.Skill.Contracts.Command;

namespace IdelPog.Skill.Contracts.Error
{
    public readonly record struct SkillUpdateError
    {
        public required SkillUpdate[] SkillUpdates { get; init; }
        public required BaseError BaseError { get; init; }
    }
}