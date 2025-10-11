using IdelPog.Core.Contracts.Error;
using IdelPog.Skill.Contracts.Command;

namespace IdelPog.Skill.Contracts.Error
{
    public readonly record struct SkillCreationError
    {
        public required SkillCreation[] SkillCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}