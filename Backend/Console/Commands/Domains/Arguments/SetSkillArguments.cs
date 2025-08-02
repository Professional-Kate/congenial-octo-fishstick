using IdelPog.Common.Enums;

namespace Console.Commands.Domains.Arguments
{
    public readonly record struct SetSkillArguments
    {
        public required SkillID SkillID { get; init; }
    }
}