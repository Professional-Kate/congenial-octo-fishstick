using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Console.Resolver.Skill
{
    public readonly record struct SetSkillArguments
    {
        public required SkillID SkillID { get; init; }
    }
}