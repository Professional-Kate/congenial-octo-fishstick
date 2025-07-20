using IdelPog.Common.Enums;

namespace Console.Types
{
    public readonly record struct SkillChangeArguments
    {
        public required SkillID SkillID { get; init; }
    }
}