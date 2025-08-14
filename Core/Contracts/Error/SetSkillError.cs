using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Error
{
    public readonly record struct SetSkillError
    {
        public required SetSkill SetSkill { get; init; }
        public required BaseError BaseError { get; init; }
    }
}