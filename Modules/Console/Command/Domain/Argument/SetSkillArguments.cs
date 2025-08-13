using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Console.Command.Domain.Argument
{
    public readonly record struct SetSkillArguments
    {
        public required SkillID SkillID { get; init; }
    }
}