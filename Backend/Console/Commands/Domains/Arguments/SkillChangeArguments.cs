using IdelPog.Common.Enums;

namespace Console.Commands.Domains.Arguments
{
    public readonly record struct SkillChangeArguments
    {
        public required SkillID SkillID { get; init; }
    }
}