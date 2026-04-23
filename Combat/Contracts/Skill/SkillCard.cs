namespace IdelPog.Combat.Contracts.Skill
{
    public readonly record struct SkillCard
    {
        public required SkillType SkillType { get; init; }
        public required Strategy Strategy { get; init; }
    }
}