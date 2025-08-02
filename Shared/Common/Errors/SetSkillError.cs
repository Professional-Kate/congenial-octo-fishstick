using IdelPog.Common.Commands;

namespace IdelPog.Common.Errors
{
    public readonly record struct SetSkillError
    {
        public required SetSkill SetSkill { get; init; }
        public required BaseError BaseError { get; init; }
    }
}