using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Error
{
    public readonly record struct SkillCreationError
    {
        public required SkillCreation[] SkillCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}